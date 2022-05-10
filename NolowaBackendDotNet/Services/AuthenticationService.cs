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
using NolowaBackendDotNet.Models.SNSLogin;
using NolowaBackendDotNet.Models.SNSLogin.Google;
using NolowaBackendDotNet.Models.SNSLogin.Kakao;
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

        string GetAuthorizationRequestURI();
        Task<AccountDTO> LoginWithUserInfo<TResponse>(string code) where TResponse : SNSUserResponseBase;
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

        public string GetAuthorizationRequestURI()
        {
            return SnsLoginProvider.GetAuthorizationRequestURI();
        }

        public async Task<AccountDTO> LoginWithUserInfo<TResponse>(string code) where TResponse : SNSUserResponseBase
        {
            _logger.LogStartTrace();

            bool hasAccessToken = await SnsLoginProvider.SetAccessTokenAsync(code);

            if(hasAccessToken == false)
                return null;

            var userInfo = await SnsLoginProvider.GetUserInfoAsync<TResponse>();

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
