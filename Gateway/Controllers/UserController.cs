using Microsoft.AspNetCore.Mvc;
using NolowaNetwork.System;
using SharedLib.Constants;
using SharedLib.Messages;
using SharedLib.Models;

namespace Gateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IMessageMaker _messageMaker;

        public UserController(IMessageBroker messageBroker, IMessageMaker messageMaker)
        {
            _messageBroker = messageBroker;
            _messageMaker = messageMaker;
        }

        [HttpPost("v1/save")]
        public async Task<User> Save(SignUpReq signUpReq)
        {
            var saveMessage = _messageMaker.MakeTakeMessage<SignUpReq>(Const.GATEWAY_SERVER_NAME, Const.API_SERVER_NAME);
            saveMessage.AccountName = signUpReq.AccountName;
            saveMessage.Email = signUpReq.Email;
            saveMessage.Password = signUpReq.Password;

            var saveResponse = await _messageBroker.TakeMessageAsync<User>(saveMessage.TakeId, saveMessage, CancellationToken.None);

            if (saveResponse != null)
            {
                return null;
            }

            return saveResponse;
        }
    }
}
