using RabbitMQ.Client;
ㅊusing RabbitMQ.Client.Events;
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
        public string ExchangeName { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
        //public string RoutingKey { get; set; } = string.Empty; => "*.QueueName.*"

        public MessageQueueConnectionData(){ }
    }

    public interface IMessageQueueService
    {
        Task<bool> InitAsync(MessageQueueConnectionData data);
        Task<bool> SendMessage<T>(string target, T body);
    }

    public class MessageQueueService : IMessageQueueService
    {
        private IModel _channel;
        private string _serverName;
        private bool _isConnected;

        public MessageQueueService()
        {

        }

        public async Task<bool> InitAsync(MessageQueueConnectionData data)
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
                        SocketReadTimeout = TimeSpan.FromSeconds(10),
                        SocketWriteTimeout = TimeSpan.FromSeconds(10),
                    };

                    var connection = connectionFactory.CreateConnection();
                    _isConnected = true;

                    _channel = connection.CreateModel();
                    _channel.QueueDeclare(queue: data.QueueName, durable: true, exclusive: false);
                    _channel.QueueBind(queue: data.QueueName, exchange: data.ExchangeName, routingKey: $"*.{data.QueueName}.*");

                    var consumer = new AsyncEventingBasicConsumer(_channel);
                    consumer.Received += Comsumer_Received;

                    _channel.BasicConsume(queue: data.QueueName, autoAck: true, consumer: consumer);
                    
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            });
        }

        private Task Comsumer_Received(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received message: {0}", message);

            return Task.CompletedTask;
        }

        public async Task<bool> SendMessage<T>(string target, T body)
        {
            if (body is null || _isConnected == false)
                return false;

            _channel.BasicPublish("", target, body: Encoding.UTF8.GetBytes(body.ToString()));

            return true;
        }
    }
}
