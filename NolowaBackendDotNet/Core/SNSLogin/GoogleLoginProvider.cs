using Microsoft.Extensions.Configuration;
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

        public GoogleLoginProvider(IHttpProvider httpProvider, IConfiguration configuration) : base(httpProvider)
        {
            _configuration = configuration;
        }

        public string GetGoogleAuthorizationRequestURI()
        {
            var authorizationRequestBuilder = new StringBuilder();

            authorizationRequestBuilder.Append(@"https://accounts.google.com/o/oauth2/v2/auth");
            authorizationRequestBuilder.Append("?");
            authorizationRequestBuilder.Append("response_type=code");
            authorizationRequestBuilder.Append("&");
            authorizationRequestBuilder.Append("access_type=offline");
            authorizationRequestBuilder.Append("&");
            //authorizationRequestBuilder.Append("scope=email%20profile");
            authorizationRequestBuilder.Append(@"scope=https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/plus.me+https://www.googleapis.com/auth/userinfo.profile");
            authorizationRequestBuilder.Append("&");
            authorizationRequestBuilder.Append($"redirect_uri={_configuration.GetValue<string>("SocialLoginGroup:GoogleLoginOption:RedirectURI")}");
            authorizationRequestBuilder.Append("&");
            authorizationRequestBuilder.Append($"client_id={_configuration.GetValue<string>("SocialLoginGroup:GoogleLoginOption:ClientID")}");

            return authorizationRequestBuilder.ToString();
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
