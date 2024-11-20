using Autofac;
using NolowaBackendDotNet.Services;
using NolowaNetwork.System;
using SharedLib.Constants;
using SharedLib.Messages;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.MessageQueue
{
    public class MessageHandler : NolowaNetwork.System.Worker.IMessageHandler
    {
        //private readonly IMessageMaker _messageMaker;
        //private readonly IMessageBroker _messageBroker;

        //public MessageHandler(IMessageMaker messageMaker, IMessageBroker messageBroker)
        //{
        //    _messageMaker = messageMaker;
        //    _messageBroker = messageBroker;
        //}

        private readonly ILifetimeScope _lifetimeScope;

        public MessageHandler(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public async Task HandleAsync(dynamic message, CancellationToken cancellationToken)
        {
            await HandleAsync(message, cancellationToken);
        }

        public async Task HandleAsync(LoginReq message, CancellationToken cancellationToken)
        {
            var messageMaker = _lifetimeScope.Resolve<IMessageMaker>();
            var messageBroker = _lifetimeScope.Resolve<IMessageBroker>();

            var accountService = InstanceResolver.Instance.Resolve<IAccountsService>();

            var response = await accountService.LoginAsync(message.Id, message.Password);

            var responseMessage = messageMaker.MakeResponseMessage<LoginRes>(Const.API_SERVER_NAME, message);
            responseMessage.Id = response.Id;
            responseMessage.UserId = response.UserId;
            responseMessage.Password = response.Password;
            responseMessage.Email = response.Email;

            await messageBroker.SendMessageAsync(responseMessage, cancellationToken);
        }
    }
}
