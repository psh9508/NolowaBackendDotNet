using Autofac;
using NolowaBackendDotNet.Services;
using NolowaNetwork.Models.Message;
using NolowaNetwork.System;
using SharedLib.Constants;
using SharedLib.Messages;
using SharedLib.Models;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.MessageQueue
{
    public class MessageHandler : NolowaNetwork.System.Worker.IMessageHandler
    {
        private readonly Lazy<IMessageMaker> _messageMaker;
        private readonly Lazy<IMessageBroker> _messageBroker;

        public MessageHandler(Lazy<IMessageMaker> messageMaker, Lazy<IMessageBroker> messageBroker)
        {
            _messageMaker = messageMaker;
            _messageBroker = messageBroker;
        }

        public async Task HandleAsync(dynamic message, CancellationToken cancellationToken)
        {
            await HandleAsync(message, cancellationToken);
        }

        public async Task HandleAsync(LoginReq message, CancellationToken cancellationToken)
        {
            var accountService = InstanceResolver.Instance.Resolve<IAccountsService>();

            var response = await accountService.LoginAsync(message.Id, message.Password);

            var responseMessage = _messageMaker.Value.MakeResponseMessage<LoginRes>(Const.API_SERVER_NAME, message);
            responseMessage.Id = response.Id;
            responseMessage.UserId = response.UserId;
            responseMessage.Password = response.Password;
            responseMessage.Email = response.Email;

            await _messageBroker.Value.SendMessageAsync(responseMessage, cancellationToken);
        }

        public async Task HandleAsync(SignUpReq message, CancellationToken cancellationToken)
        {
            var accountService = InstanceResolver.Instance.Resolve<IAccountsService>();

            var response = await accountService.SaveAsync(new()
            {
                AccountName = message.AccountName,
                Email = message.Email,
                Password = message.Password,
                //ProfileImage = 
            });

            var responseMessage = _messageMaker.Value.MakeResponseMessage<User>(Const.API_SERVER_NAME, message);
            responseMessage.Id = response.Id;
            responseMessage.UserId = response.UserId;
            responseMessage.AccountName = response.AccountName;
            responseMessage.Email = response.Email;
            responseMessage.Jwt = response.JWTToken;

            await _messageBroker.Value.SendMessageAsync(responseMessage, cancellationToken);
        }
    }
}
