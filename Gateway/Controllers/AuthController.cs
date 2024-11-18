using Microsoft.AspNetCore.Mvc;
using NolowaFrontend.Models.Protos.Generated.prot;
using NolowaNetwork.System;

namespace Gateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IMessageMaker _messageMaker;

        public AuthController(IMessageBroker messageBroker, IMessageMaker messageMaker)
        {
            _messageBroker = messageBroker;
            _messageMaker = messageMaker;
        }

        [HttpPost("v1/login")]
        public async Task LoginAsync(string id, string password)
        {
            var loginMessage = _messageMaker.MakeStartMessage<LoginReq>("gateway", "apiserver");

            var loginResponse = await _messageBroker.TakeMessageAsync<LoginRes>(loginMessage.TakeId, loginMessage, CancellationToken.None);

            if (loginResponse is null)
            {
                // 에러
            }
        }
    }
}
