using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.SNSLogin.Base
{
    public interface ISNSLogin
    {
        string GetAuthorizationRequestURI();
        Task<bool> SetAccessTokenAsync(string code);
        Task<TResponse> GetUserInfoAsync<TResponse>();
    }
}
