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
        Task<int> GetUnreadMessageCountAsync(long userId);
        Task<int> GetUnreadMessageCountAsync(long senderId, long receiverId);
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
            var unreadMessageIds = _context.DirectMessages.Where(x => x.ReceiverId == senderId && x.IsRead == false).Select(x => x.Id);

            var senderDialog = _context.DirectMessages.Where(x => x.SenderId == senderId && x.ReceiverId == receiverId)
                                                      .OrderByDescending(x => x.InsertTime)
                                                      .Take(unreadMessageIds.Count() >= 10 ? unreadMessageIds.Count() : 10);

            var receiverDialog = _context.DirectMessages.Where(x => x.SenderId == receiverId && x.ReceiverId == senderId)
                                                        .OrderByDescending(x => x.InsertTime)
                                                        .Take(unreadMessageIds.Count() >= 10 ? unreadMessageIds.Count() : 10);

            var senderReceiverDialog = senderDialog.Concat(receiverDialog)
                                                   .OrderByDescending(x => x.InsertTime);

            // 메시지 읽음 표시 // 이 함수에서 읽음 표시를 하는게 함수의 이름에서 유추 될 수 있을까?
            await MakeMessagesReadAsync(senderReceiverDialog, unreadMessageIds);

            await _context.SaveChangesAsync();

            return senderReceiverDialog.Take(unreadMessageIds.Count() + 10)
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

        public async Task<int> GetUnreadMessageCountAsync(long userId)
        {
            return await _context.DirectMessages.Where(x => x.ReceiverId == userId  // 내가 받은 
                                                    && x.SenderId != x.ReceiverId   // 내가 나한테 준건 제외
                                                    && x.IsRead == false)           // 읽지 않은 것
                                                .CountAsync();
        }

        public async Task<int> GetUnreadMessageCountAsync(long senderId, long receiverId)
        {
            return await _context.DirectMessages.Where(x => x.ReceiverId == senderId && x.SenderId == receiverId && x.IsRead == false)
                                                .CountAsync();
        }

        private async Task MakeMessagesReadAsync(IOrderedQueryable<DirectMessage> senderReceiverDialog, IQueryable<long> unreadMessageCountIds)
        {
            senderReceiverDialog.Foreach(x => {
                x.IsRead = true;
            });

            await _context.DirectMessages.Where(x => unreadMessageCountIds.Contains(x.Id))
                                         .ForEachAsync(x => x.IsRead = true);

            // 함수 밖에서 한번에 처리
            //await _context.SaveChangesAsync();
        }
    }
}
