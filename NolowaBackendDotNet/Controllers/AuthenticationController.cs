using Microsoft.AspNetCore.Mvc;
using NolowaBackendDotNet.Core;
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
        private readonly IHttpProvider _httpProvider;
        private readonly NolowaBackendDotNet.Services.IAuthenticationService _authenticationService;

        public AuthenticationController(IHttpProvider httpProvider, NolowaBackendDotNet.Services.IAuthenticationService authenticationService)
        {
            _httpProvider = httpProvider;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// 구글 Auth를 통해 Access를 요청하면 이곳으로 콜백이 들어온다.
        /// </summary>
        /// <param name="code">구글에게 AccessToken을 요청할 때 필요한 구글에서 발행한 code</param>
        /// <returns></returns>
        [HttpGet("Social/Google/Callback/")]
        public async Task GoogleCallback([FromQuery] string code)
        {
            await _authenticationService.CodeCallbackAsync(code);
        }
    }
}
