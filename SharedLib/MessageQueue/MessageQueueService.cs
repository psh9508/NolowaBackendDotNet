using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.MessageQueue
{
    public struct MessageQueueConnectionData
    {
        public string HostName { get; set; } = string.Empty;
        public string VirtualHostName { get; set; } = string.Empty;

        public MessageQueueConnectionData()
        {
        }
    }

    public interface IMessageQueueService
    {
        Task<bool> Connection(MessageQueueConnectionData data);
    }

    public class MessageQueueService : IMessageQueueService
    {
        private IModel _channel;
        private string _serverName;

        public MessageQueueService(string serverName)
        {
            _serverName = serverName;
        }

        public async Task<bool> Connection(MessageQueueConnectionData data)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var connectionFactory = new ConnectionFactory()
                    {
                        HostName = data.HostName,
                        VirtualHost = data.VirtualHostName,
                        UserName = "admin",
                        Password = "asdf1234",
                    };

                    var connection = connectionFactory.CreateConnection();
                    _channel = connection.CreateModel();
                    _channel.QueueDeclare(queue: _serverName, durable: true, exclusive: false);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            });
        }
    }
}
