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
using NolowaBackendDotNet.Models.DTOs;
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
        Task<AccountDTO> CodeCallbackAsync(string code);
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

        public async Task<AccountDTO> CodeCallbackAsync(string code)
        {
            _logger.LogStartTrace();

            await SnsLoginProvider.SetAccessTokenAsync(code);

            var userInfo = await SnsLoginProvider.GetUserInfoAsync<GoogleLoginUserInfoResponse>(@"https://www.googleapis.com/oauth2/v2/userinfo");

            if (userInfo.IsNull())
                return null;

            var userInDB = _context.Accounts.Where(x => x.Email == userInfo.Email).FirstOrDefault()?.ToDTO();

            if (userInDB.IsNull())
            {
                var savedAccount = await _accountService.SaveAsync(new Models.Account()
                {
                    Email = userInfo.Email,
                    AccountName = userInfo.Name,
                    AccountId = $"@{userInfo.Name}", // temp
                });

                if (savedAccount.IsNull())
                    return null;

                userInDB = savedAccount;
            }

            return userInDB;
        }
    }
}
