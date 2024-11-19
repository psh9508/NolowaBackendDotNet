using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils.Messaging;
using NolowaBackendDotNet.Services;
using NolowaNetwork.Models.Message;
using SharedLib.Messages;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.MessageQueue
{
    //public class MessageHandler : IMessageEventHandler
    public class MessageHandler : NolowaNetwork.System.Worker.IMessageHandler
    {
        public async Task HandleAsync(NetMessageBase message, CancellationToken cancellationToken)
        {
            // 타입별로 메핑을 해야한다.
            string messageType = message.MessageType;

            if(messageType == "LoginReq")
            {
                var payload = JsonSerializer.Deserialize<LoginReq>(message.JsonPayload);

                var accountService = InstanceResolver.Instance.Resolve<IAccountsService>();

                //return await accountService.LoginAsync(payload.Id, payload.Password);
            }

            throw new NotImplementedException();
        }

        //MessageMapper _messageMapper = new MessageMapper();

        public void HandleMessage<T>(T type, byte[] body)
        {
            var message = JsonSerializer.Deserialize<T>(body) as dynamic;

            HandleMessage(message);
        }

        public bool HandleMessage(IMessageSender sender, Message message)
        {
            throw new NotImplementedException();
        }

        private async Task<LoginRes> HandleMessage(SharedLib.Messages.LoginReq message)
        {
            try
            {
                var accountService = InstanceResolver.Instance.Resolve<IAccountsService>();

                return await accountService.LoginAsync(message.Id, message.Password);
                return null;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}
