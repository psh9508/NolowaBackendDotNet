﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils.Messaging;
using NolowaBackendDotNet.Core.MessageHandler;
using NolowaBackendDotNet.Services;
using NolowaFrontend.Models.Protos.Generated.prot;
using NolowaNetwork.Models.Message;
using SharedLib.MessageQueue;
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
        public Task HandleAsync(NetMessageBase message, CancellationToken cancellationToken)
        {
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
    }
}
