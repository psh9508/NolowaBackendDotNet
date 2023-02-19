using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace NolowaBackendDotNet.Core.MessageQueue
{
    public class MessagePublisher : IMessageQueue
    {
        private readonly IModel _channel;

        public MessagePublisher()
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "asdf1234",
                VirtualHost = "/",
            };

            var connection = connectionFactory.CreateConnection();

            _channel = connection.CreateModel();

            _channel.QueueDeclare("messagequeue", durable: true, exclusive: true);
        }

        public void SendMessage<T>(T message)
        {
            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            _channel.BasicPublish("", "messagequeue", body: body);
        }
    }
}
