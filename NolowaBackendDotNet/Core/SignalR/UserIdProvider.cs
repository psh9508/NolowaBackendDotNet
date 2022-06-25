using Microsoft.AspNetCore.SignalR;
using NolowaBackendDotNet.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.SignalR
{
    public class UserIdProvider : IUserIdProvider
    {
        //private readonly NolowaContext _context;

        //public UserIdProvider(NolowaContext context)
        //{
        //    _context = context;
        //}

        public string GetUserId(HubConnectionContext connection)
        {
            var test = connection.User?.FindFirst(ClaimTypes.Email)?.Value;

            return "";
        }
    }
}
