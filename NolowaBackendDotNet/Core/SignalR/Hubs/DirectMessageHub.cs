using Microsoft.AspNetCore.SignalR;
using NolowaBackendDotNet.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.SignalR.Hubs
{
    public class DirectMessageHub : Hub
    {
        private readonly NolowaContext _context;
        private readonly HubConnectionManager _hubConnectionManager;

        public DirectMessageHub(NolowaContext context)
        {
            _context = context;
            _hubConnectionManager = new HubConnectionManager();
        }

        public bool Login(long userId)
        {
            return _hubConnectionManager.AddChatConnection(userId, Context.ConnectionId);
        }

        public bool Logout(long userId)
        {
            return _hubConnectionManager.RemoveConnection(userId);
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

            string receiverConnectionId = _hubConnectionManager.GetChatConnection(receiverId);

            if(String.IsNullOrEmpty(receiverConnectionId))
            {
                await Clients.Caller.SendAsync("ReceiveDirectMessage", senderId, receiverId, "받는 사람이 없을 경우의 callback입니다.", "Time");
                // 알람 줘야 함.
            }
            else
            {
                // Invoke them at the same time
                Clients.Caller.SendAsync("ReceiveDirectMessage", senderId, receiverId, message, DateTime.Now.ToString("yyyy년 MM월 dd일 HH:mm:ss"));
                Clients.Client(receiverConnectionId).SendAsync("ReceiveDirectMessage", senderId, receiverId, message, DateTime.Now.ToString("yyyy년 MM월 dd일 HH:mm:ss"));
            }
        }
    }
}
