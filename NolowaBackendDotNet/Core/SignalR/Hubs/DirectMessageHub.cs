using Microsoft.AspNetCore.SignalR;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Services;
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
        private readonly IDirectMessageCacheService _cacheService;
        private readonly IDirectMessageService _directMessageService;

        public DirectMessageHub(NolowaContext context, IDirectMessageCacheService cacheService, IDirectMessageService directMessageService)
        {
            _context = context;
            _cacheService = cacheService;
            _directMessageService = directMessageService;
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
            await _cacheService.SaveAndQueueToSaveDisk(new Models.DirectMessage()
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                InsertTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                IsRead = senderId == receiverId, // 나에게 보낸 메시지면 읽음 표시를 바로 한다.
            });

            string callerConnectionId = _hubConnectionManager.GetChatConnection(senderId);
            string receiverConnectionId = _hubConnectionManager.GetChatConnection(receiverId);

            // Invoke them at the same time
            if(senderId != receiverId) // 자기 자신에게 보낸 메세지는 제외한다.
                _ = Clients.Client(callerConnectionId).SendAsync("ReceiveDirectMessage", senderId, receiverId, message, DateTime.Now.ToString("yyyy년 MM월 dd일 HH:mm:ss"));

            _ = Clients.Client(receiverConnectionId).SendAsync("ReceiveDirectMessage", senderId, receiverId, message, DateTime.Now.ToString("yyyy년 MM월 dd일 HH:mm:ss"));
        }

        public async Task ReadMessage(long senderId, long receiverId)
        {
            var unreadMessageCount = await _directMessageService.GetUnreadMessageCountAsync(senderId, receiverId);

            await Clients.Caller.SendAsync("ReadMessage", unreadMessageCount);
        }
    }
}
