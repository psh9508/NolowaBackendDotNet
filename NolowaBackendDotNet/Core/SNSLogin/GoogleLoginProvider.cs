using Microsoft.Extensions.Configuration;
using NolowaBackendDotNet.Core.SNSLogin.Base;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models.Configuration;
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
        protected override string AccessTokenURI => @"https://accounts.google.com/o/oauth2/v2/auth";
        protected override string UserInfoURI => @"https://www.googleapis.com/oauth2/v2/userinfo";

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
            var response = await _httpProvider.PostAsync<GoogleLoginAccessResponse, GoogleLoginAccessRequest>(AccessTokenURI, new GoogleLoginAccessRequest()
            {
                Code = code,
                ClientID = _configuration.GetValue<string>("SocialLoginGroup:GoogleLoginOption:ClientID"),
                ClientSecret = _configuration.GetValue<string>("SocialLoginGroup:GoogleLoginOption:Secret"),
                RedirectUrl = _configuration.GetValue<string>("SocialLoginGroup:GoogleLoginOption:RedirectURI"),
            });

            if (response.IsSuccess == false)
                return false; // AccessToken을 받아오는대 실패했습니다.

            _httpProvider.AddHeader("Authorization", $"Bearer {response.Body.AccessToken}");
            
            return true;
        }
    }
}
