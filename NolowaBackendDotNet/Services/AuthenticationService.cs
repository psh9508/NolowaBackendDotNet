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
        private readonly IConfiguration _configuration;
        private ISNSLogin _snsLoginProvider;

        public AuthenticationService(NolowaContext context, IHttpProvider httpProvider, IAccountsService accountsService, IConfiguration configuration)
        {
            _context = context;
            _accountService = accountsService;
            _configuration = configuration;
            _snsLoginProvider = SNSLoginProviderFactory(SNSType.Google, httpProvider);
        }

        private ISNSLogin SNSLoginProviderFactory(SNSType type, IHttpProvider httpProvider)
        {
            switch (type)
            {
                case SNSType.Google:
                    return new GoogleLoginProvider(httpProvider, _configuration);
                case SNSType.Meta:
                    return null;
                case SNSType.Kakao:
                    return null;
                default:
                    throw new InvalidOperationException($"알 수 없는 SNSLogin 구분값[{type}]으로 데이터를 만들 수 없습니다.");
            }
        }

        public string GetGoogleAuthorizationRequestURI()
        {
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
