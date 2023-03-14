using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedLib.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
        Task<bool> InitAsync(MessageQueueConnectionData data, IMessageEventHandler handler);
        Task<bool> SendMessageAsync<T>(string target, T body);
    }

    public class MessageQueueService : IMessageQueueService
    {
        private IModel _channel;
        private string _serverName;
        private bool _isConnected;
        private IMessageEventHandler _messageHandler;
        private MessageModelMapper _messageMapper = new();

        public MessageQueueService()
        {

        }

        public async Task<bool> InitAsync(MessageQueueConnectionData data, IMessageEventHandler handler)
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

                    _serverName = data.QueueName;
                    _messageHandler = handler;

                    var connection = connectionFactory.CreateConnection();

                    _channel = connection.CreateModel();
                    _channel.QueueDeclare(queue: _serverName, durable: true, exclusive: false);
                    _channel.QueueBind(queue: _serverName, exchange: data.ExchangeName, routingKey: $"*.{_serverName}.*");

                    var consumer = new EventingBasicConsumer(_channel);
                    consumer.Received += Consumer_Received;
                    
                    _channel.BasicConsume(queue: _serverName, autoAck: true, consumer: consumer);

                    _isConnected = true;

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            });
        }

        private void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            var body = e.Body.ToArray();

            if (body is null || body.Length <= 0)
                return;

            var message = Encoding.UTF8.GetString(body);

            var messageModel = JsonSerializer.Deserialize<MessageBase>(body);

            if (messageModel is null)
                throw new ArgumentException(nameof(body));

            if ((MessageDestination)Enum.Parse(typeof(MessageDestination), _serverName, true) != messageModel.Destination)
            {
                messageModel.Source = messageModel.Target.ToString().ToLower();
                messageModel.Target = messageModel.Destination;

                SendMessage(messageModel, body);

                return;
            }

            var modelType = _messageMapper.GetConcreteMessageType(messageModel.Function);

            _messageHandler.HandleMessage(modelType, body);

            Console.WriteLine("Received message: {0}", message);
        }

        public async Task<bool> SendMessageAsync<T>(string target, T body)
        {
            if (body is null || _isConnected == false)
                return false;

            _channel.BasicPublish("amq.topic", target, body: Encoding.UTF8.GetBytes(body.ToString()));

            return true;
        }

        //public void SendMessage<T>(T body) where T : MessageBase
        //{
        //    if (body is null)
        //        throw new ArgumentOutOfRangeException($"{nameof(body)} must not be null");

        //    if (_isConnected == false)
        //        throw new InvalidOperationException("It hasn't been connected yet");

        //    string routingKey = $"{body.Source}.{body.Target.ToString().ToLower()}.{body.Function}";
        //    string jsonBody = JsonSerializer.Serialize(body);

        //    _channel.BasicPublish("amq.topic", routingKey, body: Encoding.UTF8.GetBytes(jsonBody));
        //}
        public void SendMessage<T>(T model, byte[] body) where T : MessageBase
        {
            if (body is null)
                throw new ArgumentOutOfRangeException($"{nameof(body)} must not be null");

            if (_isConnected == false)
                throw new InvalidOperationException("It hasn't been connected yet");

            // 여기서 라우팅키가 body의 것과 다르다
            string routingKey = $"{model.Source}.{model.Target.ToString().ToLower()}.{model.Function}";
            //string jsonBody = JsonSerializer.Serialize(body);

            //_channel.BasicPublish("amq.topic", routingKey, body: Encoding.UTF8.GetBytes(jsonBody));
            _channel.BasicPublish("amq.topic", routingKey, body: body);
        }
    }
}
