using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.Hubs
{
    public class DirectMessageHub : Hub
    {
        public async Task SendMessage(long user, string message)
        {
            await Clients.Caller.SendAsync("ReceiveDirectMessage", user, message);
        }
    }
}
