using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NolowaBackendDotNet.Context;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Core.SNSLogin;
using NolowaBackendDotNet.Core.SNSLogin.Base;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models.SNSLogin.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Services
{
    public interface IAuthenticationService
    {
        string GetGoogleAuthorizationRequestURI();
        Task CodeCallbackAsync(string code);
        //Task SetAccessTokenAsync(string code);
        //Task<TResponse> GetUserInfoAsync<TResponse>(string uri);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly NolowaContext _context;
        private readonly IAccountsService _accountService;
        private ISNSLogin _snsLoginProvider;

        public AuthenticationService(NolowaContext context, IHttpProvider httpProvider, IAccountsService accountsService)
        {
            _context = context;
            _accountService = accountsService;
        }

        public string GetGoogleAuthorizationRequestURI()
        {
            _snsLoginProvider = new GoogleLoginProvider();

            return _snsLoginProvider.GetGoogleAuthorizationRequestURI();
        }

        public async Task CodeCallbackAsync(string code)
        {
            await _snsLoginProvider.SetAccessTokenAsync(code);

            var userInfo = await _snsLoginProvider.GetUserInfoAsync<GoogleLoginUserInfoResponse>(@"https://www.googleapis.com/oauth2/v2/userinfo");

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
        } 

        private async Task SetAccessTokenAsync(string code)
        {
            await _snsLoginProvider.SetAccessTokenAsync(code);
        }

        private async Task<TResponse> GetUserInfoAsync<TResponse>(string uri)
        {
            return await _snsLoginProvider.GetUserInfoAsync<TResponse>(@"https://www.googleapis.com/oauth2/v2/userinfo");
        }
    }
}
