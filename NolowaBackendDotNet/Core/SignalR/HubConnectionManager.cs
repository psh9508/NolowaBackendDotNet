using Microsoft.AspNetCore.SignalR;
using NolowaBackendDotNet.Context;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.SignalR
{
    public class HubConnectionManager 
    {
        private static readonly ConcurrentDictionary<long, string> _longConnections = new ConcurrentDictionary<long, string>();
        private static readonly ConcurrentDictionary<long, string> _chatConnections = new ConcurrentDictionary<long, string>();

        public HubConnectionManager()
        {

        }

        public bool AddLoginConnection(long userId, string connectionId)
        {
            if (_longConnections.ContainsKey(userId))
                _longConnections.TryRemove(userId, out string _);

            return _longConnections.TryAdd(userId, connectionId);
        }

        public bool AddChatConnection(long userId, string connectionId)
        {
            if (_chatConnections.ContainsKey(userId))
                _chatConnections.TryRemove(userId, out string _);

            return _chatConnections.TryAdd(userId, connectionId);
        }

        public bool RemoveConnection(long userId)
        {
            return _chatConnections.TryRemove(userId, out string value);
        }

        public string GetChatConnection(long userId)
        {
            if (_chatConnections.TryGetValue(userId, out string value))
                return value;

            return string.Empty;
        }
    }
}
