using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Mvc;
using NolowaBackendDotNet.Models;
using NolowaNetwork.System;
using SharedLib.Constants;
using SharedLib.Messages;

namespace Gateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IMessageMaker _messageMaker;

        public PostController(IMessageBroker messageBroker, IMessageMaker messageMaker)
        {
            _messageBroker = messageBroker;
            _messageMaker = messageMaker;
        }

        [HttpPost("New")]
        public async Task<NewPostRes> InsertNewPost(NewPostReq newPost)
        {
            var newPostMessage = _messageMaker.MakeTakeMessage<NewPostReq>(Const.GATEWAY_SERVER_NAME, Const.API_SERVER_NAME);
            newPostMessage.USN = newPost.USN;
            newPostMessage.Message = newPost.Message;

            var newPostResponse = await _messageBroker.TakeMessageAsync<NewPostRes>(newPostMessage.TakeId, newPostMessage, CancellationToken.None);

            if (newPostResponse == null)
            {
                return null;
            }

            return newPostResponse;
        }
    }
}
