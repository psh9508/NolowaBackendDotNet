using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost("v1/login")]
        public async Task LoginAsync(string temp)
        {
            // rabbitmq로 서버를 들려서 로그인 결과를 받아와야 함
        }
    }
}
