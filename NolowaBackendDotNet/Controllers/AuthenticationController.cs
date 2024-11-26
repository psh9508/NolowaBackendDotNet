using Microsoft.AspNetCore.Mvc;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Core.Base;
using NolowaBackendDotNet.Core.SNSLogin;
using NolowaBackendDotNet.Core.SNSLogin.Base;
using NolowaBackendDotNet.Models.SNSLogin.Google;
using NolowaBackendDotNet.Models.SNSLogin.Kakao;
using SharedLib.Dynamodb.Models;
using System;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : NolowaController
    {
        private readonly IHttpProvider _httpProvider;
        private readonly Services.IAuthenticationService _authenticationService;
        
        public AuthenticationController(IHttpProvider httpProvider, Services.IAuthenticationService authenticationService)
        {
            _httpProvider = httpProvider;
            _authenticationService = authenticationService;
        }

        [HttpGet("Social/{snsProviderName}/AuthorizationRequestURI")]
        public string GetGoogleAuthorizationRequestURI(string snsProviderName)
        {
            _authenticationService.SnsLoginProvider = SNSLoginProviderFactory(snsProviderName);
            return _authenticationService.GetAuthorizationRequestURI();
        }

        [HttpGet("Social/Google/Login")]
        public async Task<DdbUser> GoogleSocialLogin([FromQuery] string code)
        {
            _authenticationService.SnsLoginProvider = new GoogleLoginProvider(_httpProvider);
            return await _authenticationService.LoginWithUserInfo<GoogleLoginUserInfoResponse>(code);
        }

        [HttpGet("Social/Kakao/Login")]
        public async Task<DdbUser> KakaoSocialLogin([FromQuery] string code)
        {
            _authenticationService.SnsLoginProvider = new KakaoLoginProvider(_httpProvider);
            return await _authenticationService.LoginWithUserInfo<KakaoLoginUserInfoResponse>(code);
        }

        private ISNSLogin SNSLoginProviderFactory(string snsProviderName)
        {
            switch (snsProviderName.ToLower())
            {
                case "google":
                    return new GoogleLoginProvider(_httpProvider);
                case "kakao":
                    return new KakaoLoginProvider(_httpProvider);
                default:
                    throw new InvalidOperationException($"구현되지 않은 [{snsProviderName}] SNS Login Provider의 생성을 시도하였습니다.");
            }
        }
    }
}
