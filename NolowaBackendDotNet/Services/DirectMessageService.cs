using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Models.IF;
using NolowaBackendDotNet.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface IDirectMessageService
    {
        Task<IEnumerable<DirectMessage>> GetDialogAsync(long senderId, long receiverId);
        Task<IEnumerable<PreviousDialogListItem>> GetPreviousDialogList(long senderId);
        Task<int> GetUnreadMessageCount(long userId);
    }

    public class DirectMessageService : ServiceBase<DirectMessageService>, IDirectMessageService
    {
        private readonly NolowaContext context;

        public DirectMessageService(NolowaContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<DirectMessage>> GetDialogAsync(long senderId, long receiverId)
        {
            var senderDialog = await _context.DirectMessages.Where(x => x.SenderId == senderId && x.ReceiverId == receiverId)
                                                            .OrderByDescending(x => x.InsertTime)
                                                            .Take(10)
                                                            .ToListAsync();

            var receiverDialog = await _context.DirectMessages.Where(x => x.SenderId == receiverId && x.ReceiverId == senderId)
                                                              .OrderByDescending(x => x.InsertTime)
                                                              .Take(10)
                                                              .ToListAsync();
            
            var senderReceiverDialog = senderDialog.Concat(receiverDialog)
                                                   .OrderByDescending(x => x.InsertTime);

            // 메시지 읽음 표시
            senderReceiverDialog.Foreach(x => {
                x.IsRead = true;
            });

            await _context.SaveChangesAsync();

            var unreadMessageCount = senderReceiverDialog.Count(x => x.IsRead == false);

            return senderReceiverDialog.Take(unreadMessageCount + 10)
                                       .AsEnumerable()
                                       .OrderBy(x => x.InsertTime);
        }

        public async Task<IEnumerable<PreviousDialogListItem>> GetPreviousDialogList(long senderId)
        {
            //SELECT TOP 10 *
		    //FROM ( SELECT MAIN.ID, MAIN.INSERT_TIME
		    //       FROM (SELECT G.*, RECEIVER_ID, SENDER_ID, INSERT_TIME	  	
	        //             FROM (SELECT MAX(ID) AS ID
		    //       	             , SUM(CASE(IS_READ) WHEN 1 THEN 0 ELSE 1 END) AS NEW_MESSAGE_CNT
		    //                   FROM[Nolowa].[dbo].[DirectMessage]
		    //                   WHERE SENDER_ID = 2
		    //                   GROUP BY RECEIVER_ID) AS G
	        //             JOIN [Nolowa].[dbo].[DirectMessage] AS M ON M.ID = G.ID AND M.RECEIVER_ID = 3) AS MAIN
		    //       JOIN [dbo].[Account] AS A ON A.ID = MAIN.RECEIVER_ID			
	        //	   UNION ALL
		    //	   SELECT MAIN.ID, MAIN.INSERT_TIME
		    //       FROM (SELECT G.*, RECEIVER_ID, SENDER_ID, INSERT_TIME	  
	        //             FROM (SELECT MAX(ID) AS ID
		    //       	             , SUM(CASE(IS_READ) WHEN 1 THEN 0 ELSE 1 END) AS NEW_MESSAGE_CNT
		    //                   FROM[Nolowa].[dbo].[DirectMessage]
		    //                   WHERE SENDER_ID = 2
		    //                   GROUP BY RECEIVER_ID) AS G
	        //             LEFT JOIN [Nolowa].[dbo].[DirectMessage] AS M ON M.ID = G.ID AND M.SENDER_ID = 2) AS MAIN
		    //       JOIN [dbo].[Account] AS A ON A.ID = MAIN.SENDER_ID
		    //	  ) LAST
		    //ORDER BY INSERT_TIME DESC

            var previousDialogList = new List<PreviousDialogListItem>();

            var receivers = _context.DirectMessages.Where(x => x.SenderId == senderId)
                                                   .Select(x => x.ReceiverId)
                                                   .Distinct().ToList();

            foreach (var receiver in receivers)
            {
                var send = _context.DirectMessages.Where(x => x.SenderId == senderId && x.ReceiverId == receiver)
                                                    .GroupBy(dm => dm.ReceiverId)
                                                    .Select(x => new
                                                    {
                                                        Id = x.Max(x => x.Id),
                                                        NewMessageCount = x.Where(x => x.ReceiverId == senderId).Sum(x => x.IsRead ? 0 : 1)
                                                    })
                                                    .Join(_context.DirectMessages, x => x.Id, g => g.Id, (x, g) =>
                                                        new
                                                        {
                                                            ReceiverId = g.ReceiverId,
                                                            Message = g.Message,
                                                            Time = g.InsertTime,
                                                            NewMessageCount = x.NewMessageCount,
                                                        }
                                                    ).Join(_context.Accounts, x => x.ReceiverId, a => a.Id, (x, a) =>
                                                        new PreviousDialogListItem()
                                                        {
                                                            Account = _mapper.Map<AccountDTO>(new Account()
                                                            {
                                                                Id = a.Id,
                                                                UserId = a.UserId,
                                                                AccountName = a.AccountName,
                                                                ProfileInfo = new ProfileInfo()
                                                                {
                                                                    ProfileImg = a.ProfileInfo.ProfileImg,
                                                                },
                                                            }),
                                                            Message = x.Message,
                                                            Time = x.Time,
                                                            NewMessageCount = x.NewMessageCount,
                                                        }
                                                    ).ToList().SingleOrDefault();

                var received = _context.DirectMessages.Where(x => x.SenderId == receiver && x.ReceiverId == senderId)
                                                        .GroupBy(dm => dm.SenderId)
                                                        .Select(x => new
                                                        {
                                                            Id = x.Max(x => x.Id),
                                                            NewMessageCount = x.Where(x => x.ReceiverId == senderId).Sum(x => x.IsRead ? 0 : 1)
                                                        })
                                                        .Join(_context.DirectMessages, x => x.Id, g => g.Id, (x, g) =>
                                                            new
                                                            {
                                                                SenderId = g.SenderId,
                                                                Message = g.Message,
                                                                Time = g.InsertTime,
                                                                NewMessageCount = x.NewMessageCount,
                                                            }
                                                        ).Join(_context.Accounts, x => x.SenderId, a => a.Id, (x, a) =>
                                                            new PreviousDialogListItem()
                                                            {
                                                                Account = _mapper.Map<AccountDTO>(new Account()
                                                                {
                                                                    Id = a.Id,
                                                                    UserId = a.UserId,
                                                                    AccountName = a.AccountName,
                                                                    ProfileInfo = new ProfileInfo()
                                                                    {
                                                                        ProfileImg = a.ProfileInfo.ProfileImg,
                                                                    },
                                                                }),
                                                                Message = x.Message,
                                                                Time = x.Time,
                                                                NewMessageCount = x.NewMessageCount,
                                                            }
                                                             ).ToList().SingleOrDefault();

                var sendTime = send?.Time;
                var receivedTime = received?.Time;

                var addedData = sendTime.CompareTo(receivedTime) > 0 ? send : received;

                previousDialogList.Add(addedData);
            }

            return previousDialogList;
        }

        public async Task<int> GetUnreadMessageCount(long userId)
        {
            return await _context.DirectMessages.Where(x => x.ReceiverId == userId && x.IsRead == false)
                                                .CountAsync();
        }
    }
}
