﻿using Microsoft.Extensions.Configuration;
using NolowaBackendDotNet.Core.SNSLogin.Base;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models.SNSLogin.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.SNSLogin
{
    public class GoogleLoginProvider : SNSLoginBase, ISNSLogin
    {
        private readonly IConfiguration _configuration;

        public GoogleLoginProvider()
        {
            _configuration = InstanceResolver.Instance.Resolve<IConfiguration>();
        }

        public string GetAuthorizationRequestURI()
        {
            return GetQueryString("https://accounts.google.com/o/oauth2/v2/auth", new Dictionary<string, string>()
            {
                ["response_type"] = "code",
                ["access_type"] = "offline",
                ["scope"] = @"https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/plus.me+https://www.googleapis.com/auth/userinfo.profile",
                ["redirect_uri"] = _configuration.GetValue<string>("SocialLoginGroup:GoogleLoginOption:RedirectURI"),
                ["client_id"] = _configuration.GetValue<string>("SocialLoginGroup:GoogleLoginOption:ClientID"),
            });
        }

        public async Task<bool> SetAccessTokenAsync(string code)
        {
            var response = await _httpProvider.PostAsync<GoogleLoginAccessResponse, GoogleLoginAccessRequest>(@"https://oauth2.googleapis.com/token", new GoogleLoginAccessRequest()
            {
                Code = code,
                ClientID = _configuration.GetValue<string>("SocialLoginGroup:GoogleLoginOption:ClientID"),
                ClientSecret = _configuration.GetValue<string>("SocialLoginGroup:GoogleLoginOption:Secret"),
                RedirectUrl = _configuration.GetValue<string>("SocialLoginGroup:GoogleLoginOption:RedirectURI"),
            });

            if (response.IsNull())
                return false; // AccessToken을 받아오는대 실패했습니다.

            _httpProvider.AddHeader("Authorization", $"Bearer {response.AccessToken}");
            
            return true;
        }

        public async Task<TResponse> GetUserInfoAsync<TResponse>(string uri)
        {
            if (_httpProvider.HasHeader("Authorization") == false)
                throw new InvalidOperationException("AccessToken을 먼저 발급 받아야 합니다.");

            var userInfo = await _httpProvider.GetAsync<TResponse>(uri);

            return userInfo;
        }
    }
}
