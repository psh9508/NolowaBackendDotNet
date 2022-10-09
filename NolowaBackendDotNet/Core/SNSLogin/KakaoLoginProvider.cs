using Microsoft.Extensions.Configuration;
using NolowaBackendDotNet.Core.SNSLogin.Base;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models.Configuration;
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
        protected override string AuthenticationPageURI => @"https://kauth.kakao.com/oauth/authorize";
        protected override string AccessTokenURI => @"https://kauth.kakao.com/oauth/token";
        protected override string UserInfoURI => @"https://kapi.kakao.com/v2/user/me";

        private readonly IConfiguration _configuration;

        public KakaoLoginProvider(IHttpProvider httpProvider) : base(httpProvider)
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
            var response = await _httpProvider.PostAsync<KakaoLoginAccessResponse, KakaoLoginAccessRequest>(AccessTokenURI, new KakaoLoginAccessRequest()
            {
                Code = code,
                ClientID = _configuration.GetValue<string>("SocialLoginGroup:KakaoLoginOption:RestAPIKey"),
                RedirectUrl = _configuration.GetValue<string>("SocialLoginGroup:KakaoLoginOption:RedirectURI"),
            }, "application/x-www-form-urlencoded");

            if (response.IsSuccess == false)
                return false; // AccessToken을 받아오는대 실패했습니다.

            _httpProvider.AddHeader("Authorization", $"Bearer {response.Body.AccessToken}");

            return true;
        }
    }
}
