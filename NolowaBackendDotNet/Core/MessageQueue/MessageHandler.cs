using NolowaBackendDotNet.Models.DTOs;
using NolowaBackendDotNet.Services;
using NolowaNetwork.System;
using SharedLib.Constants;
using SharedLib.Messages;
using SharedLib.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.MessageQueue
{
    public class MessageHandler : NolowaNetwork.System.Worker.IMessageHandler
    {
        private readonly Lazy<IMessageMaker> _messageMaker;
        private readonly Lazy<IMessageBroker> _messageBroker;
        private readonly Lazy<IAccountsService> _accountService;
        private readonly Lazy<IPostsService> _postService;

        public MessageHandler(
            Lazy<IMessageMaker> messageMaker,
            Lazy<IMessageBroker> messageBroker,
            Lazy<IAccountsService> accountService,
            Lazy<IPostsService> postService)
        {
            _messageMaker = messageMaker;
            _messageBroker = messageBroker;
            _accountService = accountService;
            _postService = postService;
        }

        public async Task HandleAsync(dynamic message, CancellationToken cancellationToken)
        {
            await HandleAsync(message, cancellationToken);
        }

        public async Task HandleAsync(LoginReq message, CancellationToken cancellationToken)
        {
            var response = await _accountService.Value.LoginAsync(message.Id, message.Password);

            if(response is null)
            {
                //await _messageBroker.Value.SendMessageAsync(new()
                //{
                    
                //}, cancellationToken);
                return;
            }

            var responseMessage = _messageMaker.Value.MakeResponseMessage<LoginRes>(Const.API_SERVER_NAME, message);
            responseMessage.USN= response.USN;
            responseMessage.AccountName = response.AccountName;
            responseMessage.Password = response.Password;
            responseMessage.Email = response.Email;
            responseMessage.Jwt = response.Jwt;

            await _messageBroker.Value.SendMessageAsync(responseMessage, cancellationToken);
        }

        public async Task HandleAsync(SignUpReq message, CancellationToken cancellationToken)
        {
            var response = await _accountService.Value.SaveAsync(new()
            {
                AccountName = message.AccountName,
                Email = message.Email,
                Password = message.Password,
                //ProfileImage = 
            });

            var responseMessage = _messageMaker.Value.MakeResponseMessage<User>(Const.API_SERVER_NAME, message);
            //responseMessage.Id = response.Id;
            responseMessage.USN = response.USN;
            responseMessage.UserId = response.UserId;
            responseMessage.AccountName = response.AccountName;
            responseMessage.Email = response.Email;

            await _messageBroker.Value.SendMessageAsync(responseMessage, cancellationToken);
        }

        public async Task HandleAsync(NewPostReq message, CancellationToken cancellationToken)
        {
            var response = _postService.Value.InsertPostAsync(message);

            var responseMessage = _messageMaker.Value.MakeResponseMessage<NewPostRes>(Const.API_SERVER_NAME, message);
            //responseMessage.

            await _messageBroker.Value.SendMessageAsync(responseMessage, cancellationToken);
        }
    }
}
