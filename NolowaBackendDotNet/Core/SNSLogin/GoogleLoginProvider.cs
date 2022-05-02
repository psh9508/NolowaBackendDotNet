using NolowaBackendDotNet.Core.SNSLogin.Base;
using NolowaBackendDotNet.Extensions;
using NolowaBackendDotNet.Models.SNSLogin.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.SNSLogin
{
    public class GoogleLoginProvider : SNSLoginBase, ISNSLogin
    {
        public GoogleLoginProvider(IHttpProvider httpProvider) : base(httpProvider)
        {

        }

        public async Task<bool> SetAccessTokenAsync(string code)
        {
            var response = await _httpProvider.PostAsync<GoogleLoginAccessResponse, GoogleLoginAccessRequest>(@"https://oauth2.googleapis.com/token", new GoogleLoginAccessRequest()
            {
                Code = code,
                ClientID = "설정파일에서 가져올 수 있도록 할 예정",
                ClientSecret = "설정파일에서 가져올 수 있도록 할 예정",
                RedirectUrl = "설정파일에서 가져올 수 있도록 할 예정",
            });

            if (response.IsNull())
                return false;

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
