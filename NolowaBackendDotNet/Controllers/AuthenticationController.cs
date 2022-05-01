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
        [HttpGet("Social/Google/Callback/")]
        public void GoogleCallback([FromQuery] string code)
        {

        }
    }
}
