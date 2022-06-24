using Microsoft.AspNetCore.SignalR;
using NolowaBackendDotNet.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.Hubs
{
    public class DirectMessageHub : Hub
    {
        private readonly NolowaContext _context;

        public DirectMessageHub(NolowaContext context)
        {
            _context = context;
        }

        public async Task SendMessage(long senderId, long receiverId, string message)
        {
            // 1차로 DB에 저장
            _context.DirectMessages.Add(new Models.DirectMessage()
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                InsertTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            });

            _context.SaveChanges();

            // 저장 후 리턴 함
            await Clients.Caller.SendAsync("ReceiveDirectMessage", senderId, receiverId, message);
            //await Clients.User(receiverId.ToString()).SendAsync("ReceiveDirectMessage", senderId, message);
        }
    }
}
