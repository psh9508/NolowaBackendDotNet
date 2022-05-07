using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NolowaBackendDotNet.Core;
using NolowaBackendDotNet.Core.Base;
using NolowaBackendDotNet.Core.SNSLogin;
using NolowaBackendDotNet.Models.DTOs;
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

        public AuthenticationController(IHttpHeader httpHeader, Services.IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("Social/Google/AuthorizationRequestURI")]
        public string GetGoogleAuthorizationRequestURI()
        {
            _authenticationService.SnsLoginProvider = new GoogleLoginProvider();
            return _authenticationService.GetGoogleAuthorizationRequestURI();
        }

        /// <summary>
        /// 구글 Auth를 통해 Access를 요청하면 이곳으로 콜백이 들어온다.
        /// </summary>
        /// <param name="code">구글에게 AccessToken을 요청할 때 필요한 구글에서 발행한 code</param>
        /// <returns></returns>
        [HttpGet("Social/Google/Login")]
        public async Task<AccountDTO> GoogleCallback([FromQuery] string code)
        {
            _authenticationService.SnsLoginProvider = new GoogleLoginProvider();
            return await _authenticationService.CodeCallbackAsync(code);
        }
    }
}
