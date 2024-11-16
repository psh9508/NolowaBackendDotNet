using Microsoft.AspNetCore.Mvc;
using NolowaNetwork.Protocols.Http;

namespace Gateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpProvider _httpProvider;

        public AuthController(IHttpProvider httpProvider)
        {
            _httpProvider = httpProvider;
        }

        [HttpPost("v1/login")]
        public async Task LoginAsync(string temp)
        {
            // rabbitmq로 서버를 들려서 로그인 결과를 받아와야 함
            
        }
    }
}
