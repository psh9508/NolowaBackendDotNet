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
        Task<int> SetReadAllMessageAsync(long senderId, long receiverId);
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

        public async Task<IEnumerable<PreviousDialogListItem>> GetPreviousDialogList(long loginUserId)
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

            var idGroups = await _context.DirectMessages.Where(x => x.SenderId == loginUserId || x.ReceiverId == loginUserId) // 내가 보낸 것과 내가 받은 것 모두 가져온다.
                                                        .GroupBy(x => new { x.SenderId, x.ReceiverId }, (key, group) => new
                                                        {
                                                            SenderId = key.SenderId,
                                                            ReceiverId = key.ReceiverId,
                                                        })
                                                        .ToListAsync();

            var messageDataCollection = new List<DirectMessage>();

            // 내가 주고 받은 메시지를 가져와 최신 시간으로 삽입된 데이터만 추출한다.
            foreach (var ids in idGroups)
            {
                var message = await _context.DirectMessages.OrderByDescending(x => x.InsertTime)
                                                           .FirstOrDefaultAsync(x => x.SenderId == ids.SenderId && x.ReceiverId == ids.ReceiverId);

                if (message.IsNull())
                    continue;

                // 내가 보낸 바로 메시지면 저장하고 다음 데이터를 확인한다.
                if (ids.SenderId == ids.ReceiverId)
                {
                    messageDataCollection.Add(message);
                    continue;
                }

                var sameContextMessage = messageDataCollection.OrderByDescending(x => x.InsertTime).FirstOrDefault(x => x.SenderId == ids.ReceiverId && x.ReceiverId == ids.SenderId);

                // 기존 데이터에서 같은 사람끼리 주고 받은 데이터가 있는지 확인한다.
                if (sameContextMessage.IsNull())
                {
                    messageDataCollection.Add(message);
                    continue;
                }

                // 같은 사람끼리 주고 받은 데이터가 있더면 시간순으로 최근 것을 저장하고 기존에 저장되어 있던것을 지워준다.
                if (message.InsertTime.CompareTo(sameContextMessage.InsertTime) > 0)
                {
                    var alreadyInsertedMessage = messageDataCollection.Single(x => x.SenderId == ids.ReceiverId && x.ReceiverId == ids.SenderId);
                    messageDataCollection.Remove(alreadyInsertedMessage);

                    messageDataCollection.Add(message);
                }
            }

            var finalDialog = messageDataCollection.Join(_context.Accounts.Include(dm => dm.ProfileInfo).ThenInclude(dm => dm.ProfileImg)
                                                     , dm => dm.SenderId == loginUserId ? dm.ReceiverId : dm.SenderId
                                                     , account => account.Id
                                                     , (dm, account) => new PreviousDialogListItem()
                                                     {
                                                         Account = _mapper.Map<AccountDTO>(new Account()
                                                         {
                                                             Id = account.Id,
                                                             UserId = account.UserId,
                                                             AccountName = account.AccountName,
                                                             ProfileInfo = new ProfileInfo()
                                                             {
                                                                 ProfileImg = account.ProfileInfo?.ProfileImg,
                                                             },
                                                         }),
                                                         Message = dm.Message,
                                                         Time = dm.InsertTime,
                                                         NewMessageCount = GetUnreadMessageCountAsync(dm.SenderId, dm.ReceiverId).Result,
                                                     }
                                                     ).OrderByDescending(x => x.Time);

            return finalDialog;
        }

        public async Task<int> GetUnreadMessageCountAsync(long userId)
        {
            return await _context.DirectMessages.Where(x => x.ReceiverId == userId  // 내가 받은 것
                                                    && x.SenderId != x.ReceiverId   // 내가 나한테 준건 제외
                                                    && x.IsRead == false)           // 읽지 않은 것
                                                .CountAsync();
        }

        public async Task<int> GetUnreadMessageCountAsync(long senderId, long receiverId)
        {
            return await _context.DirectMessages.Where(x => ((x.ReceiverId == receiverId && x.SenderId == senderId) || (x.ReceiverId == senderId && x.SenderId == receiverId)) 
                                                            && x.IsRead == false)
                                               .CountAsync();
        }

        public async Task<int> SetReadAllMessageAsync(long senderId, long receiverId)
        {
            _context.DirectMessages.Where(x => x.ReceiverId == senderId 
                                            && x.SenderId == receiverId 
                                            && x.IsRead == false)
                                   .Foreach(x => x.IsRead = true);

            return await _context.SaveChangesAsync();
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
