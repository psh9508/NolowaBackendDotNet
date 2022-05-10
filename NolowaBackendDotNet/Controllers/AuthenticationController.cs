using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Core.Base;
using NolowaBackendDotNet.Core.SNSLogin;
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

        [HttpGet("Social/Google/AuthorizationRequestURI")]
        public string GetGoogleAuthorizationRequestURI()
        {
            _authenticationService.SnsLoginProvider = new GoogleLoginProvider();
            return _authenticationService.GetAuthorizationRequestURI();
        }

        [HttpGet("Social/Google/Login")]
        public async Task<AccountDTO> GoogleSocialLogin([FromQuery] string code)
        {
            _authenticationService.SnsLoginProvider = new GoogleLoginProvider();
            return await _authenticationService.LoginWithUserInfo<GoogleLoginUserInfoResponse>(code);
        }

        [HttpGet("Social/Kakao/AuthorizationRequestURI")]
        public string GetKakaoAuthorizationRequestURI()
        {
            _authenticationService.SnsLoginProvider = new KakaoLoginProvider();
            return _authenticationService.GetAuthorizationRequestURI();
        }

        [HttpGet("Social/Kakao/Login")]
        public async Task<AccountDTO> KakaoSocialLogin([FromQuery] string code)
        {
            _authenticationService.SnsLoginProvider = new KakaoLoginProvider();
            return await _authenticationService.LoginWithUserInfo<KakaoLoginUserInfoResponse>(code);
        }
    }
}
