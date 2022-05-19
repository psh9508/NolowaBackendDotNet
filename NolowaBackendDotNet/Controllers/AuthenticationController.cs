using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Core.Base;
using NolowaBackendDotNet.Core.SNSLogin;
using NolowaBackendDotNet.Core.SNSLogin.Base;
using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Models.SNSLogin.Google;
using NolowaBackendDotNet.Models.SNSLogin.Kakao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : NolowaController
    {
        private readonly Services.IAuthenticationService _authenticationService;

        public AuthenticationController(Services.IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("Social/{snsProviderName}/AuthorizationRequestURI")]
        public string GetGoogleAuthorizationRequestURI(string snsProviderName)
        {
            _authenticationService.SnsLoginProvider = GetSNSLoginProvider(snsProviderName);
            return _authenticationService.GetAuthorizationRequestURI();
        }

        [HttpGet("Social/Google/Login")]
        public async Task<AccountDTO> GoogleSocialLogin([FromQuery] string code)
        {
            _authenticationService.SnsLoginProvider = new GoogleLoginProvider();
            return await _authenticationService.LoginWithUserInfo<GoogleLoginUserInfoResponse>(code);
        }

        [HttpGet("Social/Kakao/Login")]
        public async Task<AccountDTO> KakaoSocialLogin([FromQuery] string code)
        {
            _authenticationService.SnsLoginProvider = new KakaoLoginProvider();
            return await _authenticationService.LoginWithUserInfo<KakaoLoginUserInfoResponse>(code);
        }

        private ISNSLogin GetSNSLoginProvider(string snsProviderName)
        {
            switch (snsProviderName.ToLower())
            {
                case "google":
                    return new GoogleLoginProvider();
                case "kakao":
                    return new KakaoLoginProvider();
                default:
                    throw new InvalidOperationException($"구현되지 않은 [{snsProviderName}] SNS Login Provider의 생성을 시도하였습니다.");
            }
        }
    }
}
