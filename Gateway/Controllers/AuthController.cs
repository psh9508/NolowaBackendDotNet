using Microsoft.AspNetCore.Mvc;
using NolowaNetwork.System;
using SharedLib.Constants;

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
            var loginMessage = _messageMaker.MakeTakeMessage<SharedLib.Messages.LoginReq>(Const.GATEWAY_SERVER_NAME, Const.API_SERVER_NAME);
            loginMessage.Id = id;
            loginMessage.Password = password;

            var loginResponse = await _messageBroker.TakeMessageAsync<SharedLib.Messages.LoginRes>(loginMessage.TakeId, loginMessage, CancellationToken.None);

            if (loginResponse == null)
            {

            }

            //var loginMessage = _messageMaker.MakeStartMessage<SharedLib.Messages.LoginReq>(Const.GATEWAY_SERVER_NAME, Const.API_SERVER_NAME);
            //loginMessage.Id = id;
            //loginMessage.Password = password;

            //await _messageBroker.SendMessageAsync(loginMessage, CancellationToken.None);
        }
    }
}
