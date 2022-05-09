using Microsoft.Extensions.Configuration;
using NolowaBackendDotNet.Core.SNSLogin.Base;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models.SNSLogin.Kakao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.SNSLogin
{
    public class KakaoLoginProvider : SNSLoginBase, ISNSLogin
    {
        private readonly IConfiguration _configuration;

        public KakaoLoginProvider()
        {
            _configuration = InstanceResolver.Instance.Resolve<IConfiguration>();
        }

        public string GetAuthorizationRequestURI()
        {
            return GetQueryString("https://kauth.kakao.com/oauth/authorize", new Dictionary<string, string>()
            {
                ["response_type"] = "code",
                ["client_id"] = _configuration.GetValue<string>("SocialLoginGroup:KakaoLoginOption:RestAPIKey"),
                ["redirect_uri"] = _configuration.GetValue<string>("SocialLoginGroup:KakaoLoginOption:RedirectURI"),
            });
        }

        public async Task<bool> SetAccessTokenAsync(string code)
        {
            if (code.IsNull())
                return false;

            var response = await _httpProvider.PostAsync<KakaoLoginAccessResponse, KakaoLoginAccessRequest>(@"https://kauth.kakao.com/oauth/token", new KakaoLoginAccessRequest()
            {
                Code = code,
                ClientID = _configuration.GetValue<string>("SocialLoginGroup:KakaoLoginOption:RestAPIKey"),
                RedirectUrl = _configuration.GetValue<string>("SocialLoginGroup:KakaoLoginOption:RedirectURI"),
            }, "application/x-www-form-urlencoded");

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
