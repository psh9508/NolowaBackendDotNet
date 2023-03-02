using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils.Messaging;
using NolowaBackendDotNet.Core.MessageHandler;
using NolowaBackendDotNet.Services;
using NolowaFrontend.Models.Protos.Generated.prot;
using SharedLib.MessageQueue;
using SharedLib.Messages;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.MessageQueue
{
    public class MessageHandler : IMessageEventHandler
    {
        //MessageMapper _messageMapper = new MessageMapper();

        public void HandleMessage<T>(T type, byte[] body)
        {
            var message = JsonSerializer.Deserialize<T>(body) as dynamic;

            HandleMessage(message);
        }

        private async Task<LoginRes> HandleMessage(LoginMessage message)
        {
            try
            {
                var accountService = InstanceResolver.Instance.Resolve<IAccountsService>();

                return await accountService.LoginAsync(message.Id, message.Password);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        private void HandleMessage(string test)
        {

        }

        private void HandleMessage(int test)
        {

        }
    }
}
