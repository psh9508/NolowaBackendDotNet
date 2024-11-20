using NolowaNetwork.Models.Message;
using NolowaNetwork.System.Worker;

namespace Gateway.MessageQueue
{
    public class MessageHandler : IMessageHandler
    {
        public Task HandleAsync(dynamic message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
