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

        public MessageHandler(
            Lazy<IMessageMaker> messageMaker,
            Lazy<IMessageBroker> messageBroker,
            Lazy<IAccountsService> accountService)
        {
            _messageMaker = messageMaker;
            _messageBroker = messageBroker;
            _accountService = accountService;
        }

        public async Task HandleAsync(dynamic message, CancellationToken cancellationToken)
        {
            await HandleAsync(message, cancellationToken);
        }

        public async Task HandleAsync(LoginReq message, CancellationToken cancellationToken)
        {
            var response = await _accountService.Value.LoginAsync(message.Id, message.Password);

            var responseMessage = _messageMaker.Value.MakeResponseMessage<LoginRes>(Const.API_SERVER_NAME, message);
            responseMessage.Id = response.Id;
            responseMessage.UserId = response.UserId;
            responseMessage.Password = response.Password;
            responseMessage.Email = response.Email;

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
    }
}
