using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Core.SNSLogin;
using NolowaBackendDotNet.Core.SNSLogin.Base;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models.SNSLogin.Google;
using NolowaBackendDotNet.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface IAuthenticationService
    {
        ISNSLogin SnsLoginProvider { get; set; }

        string GetGoogleAuthorizationRequestURI();
        Task CodeCallbackAsync(string code);
        //Task SetAccessTokenAsync(string code);
        //Task<TResponse> GetUserInfoAsync<TResponse>(string uri);
    }

    public class AuthenticationService : ServiceBase<AuthenticationService>, IAuthenticationService
    {
        private readonly IAccountsService _accountService;
        public ISNSLogin SnsLoginProvider { get; set; }

        public AuthenticationService(IAccountsService accountsService)
        {
            _accountService = accountsService;
        }

        public string GetGoogleAuthorizationRequestURI()
        {
            return SnsLoginProvider.GetGoogleAuthorizationRequestURI();
        }

        public async Task CodeCallbackAsync(string code)
        {
            _logger.LogStartTrace();

            await SnsLoginProvider.SetAccessTokenAsync(code);

            var userInfo = await SnsLoginProvider.GetUserInfoAsync<GoogleLoginUserInfoResponse>(@"https://www.googleapis.com/oauth2/v2/userinfo");

            if (userInfo.IsNull())
                return;

            var hasUserInDB = _context.Accounts.Any(x => x.Email == userInfo.Email);

            if(hasUserInDB == false)
            {
                var savedAccount = await _accountService.SaveAsync(new Models.Account()
                {
                    Email = userInfo.Email,
                    AccountName = userInfo.Name,
                    AccountId = $"@{userInfo.Name}", // temp
                });

                if (savedAccount.IsNull())
                    return;
            }

            await _accountService.LoginAsync(userInfo.Email, null);

            // Client로 로그인 사실 전달 해야 함.
            
            _logger.LogEndTrace();
        } 
    }
}
