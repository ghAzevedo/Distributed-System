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
    public interface IMessageProducer<TSent, TResponseDto>
    {
        Task<TResponseDto> CallAsync(TSent obj, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class MessageSender<TSent, TResponseDto> : IMessageProducer<TSent, ResultDto<TResponseDto>>, IDisposable
        where TResponseDto : class
    {
        private readonly ISerializer _serializer;
        private readonly string _queueName;
        private readonly string _responseQueueName;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<ResultDto<TResponseDto>>> _callbackMapper =
                new ConcurrentDictionary<string, TaskCompletionSource<ResultDto<TResponseDto>>>();
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;

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
            _responseQueueName = $"{queueName}-response";

            // create response queue on RabbitMQ
            _channel.QueueDeclare(queue: _responseQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // create queue on RabbitMQ
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (model, ea) =>
            {
                if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out TaskCompletionSource<ResultDto<TResponseDto>> tcs))
                {
                    return;
                }

                var res = _serializer.DeserializeObject<ResultDto<TResponseDto>>(ea.Body.ToArray());
                tcs.TrySetResult(res);
            };
        }

        public Task<ResultDto<TResponseDto>> CallAsync(TSent obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            IBasicProperties props = _channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = _responseQueueName;
            var messageBytes = _serializer.SerializeObjectToByteArray(obj);
            var tcs = new TaskCompletionSource<ResultDto<TResponseDto>>();
            _callbackMapper.TryAdd(correlationId, tcs);

            _channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: props,
                body: messageBytes);

            _channel.BasicConsume(
                consumer: _consumer,
                queue: _responseQueueName,
                autoAck: true);

            cancellationToken.Register(() => _callbackMapper.TryRemove(correlationId, out var tmp));
            return tcs.Task;
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
