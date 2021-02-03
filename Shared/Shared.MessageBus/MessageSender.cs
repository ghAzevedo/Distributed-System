using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RetailInMotion.Model.Dto.Message;
using Shared.Utils.Serialization;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.MessageBus
{
    public interface IMessageProducer<TSent>
    {
        ResultDto<string> CallAsync(TSent obj, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class MessageSender<TSent> : IMessageProducer<TSent>, IDisposable
    {
        private readonly ISerializer _serializer;
        private readonly string _queueName;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<ResultDto<string>>> _callbackMapper =
                new ConcurrentDictionary<string, TaskCompletionSource<ResultDto<string>>>();
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageSender(RabbitMQSettings settings, string queueName, ISerializer serializer)
        {
            _serializer = serializer;
            _queueName = queueName;

            Console.WriteLine($"settings: {settings.Hostname} - {settings.Username} - {settings.Password}");

            var factory = new ConnectionFactory() 
            { 
                HostName = settings.Hostname, 
                UserName = settings.Username, 
                Password = settings.Password 
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // create queue on RabbitMQ
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public ResultDto<string> CallAsync(TSent obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            IBasicProperties props = _channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            var messageBytes = _serializer.SerializeObjectToByteArray(obj);

            _channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: props,
                body: messageBytes);

            cancellationToken.Register(() => _callbackMapper.TryRemove(correlationId, out var tmp));

            return new ResultDto<string>($"Message sent with correlation id: {correlationId}");
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
