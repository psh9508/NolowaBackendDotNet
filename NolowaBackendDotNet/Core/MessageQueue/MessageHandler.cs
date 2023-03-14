using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils.Messaging;
using NolowaBackendDotNet.Core.MessageHandler;
using NolowaBackendDotNet.Services;
using NolowaFrontend.Models.Protos.Generated.prot;
using SharedLib.MessageQueue;
using SharedLib.Messages;
using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core.MessageQueue
{
    public class MessageHandler : IMessageEventHandler
    {
        private readonly IMessageQueueService _messageService;

        public MessageHandler(IMessageQueueService messageQueueService)
        {
            _messageService = messageQueueService;
        }

        public Task HandleMessage<T>(T type, byte[] body)
        {
            var message = JsonSerializer.Deserialize<T>(body) as dynamic;

            return HandleMessage(message);
        }

        private async Task HandleMessage(LoginMessage message)
        {
            try
            {
                var accountService = InstanceResolver.Instance.Resolve<IAccountsService>();

                var result = await accountService.LoginAsync(message.Id, message.Password);

                message.Source = "server";
                message.Target = MessageDestination.GATEWAY;
                //message.Destination = message.Origin;

                // Origin에게 리턴해야함
                string originQueueName = message.Origin.Split(":")[1];
                string function = nameof(LoginMessage).ToLower();

                await _messageService.SendMessageAsync($"{message.Source}.{originQueueName}.{function}", result);
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
