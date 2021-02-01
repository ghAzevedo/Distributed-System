using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RetailInMotion.Model.Dto.Message;
using Shared.Utils.Serialization;
using Shared.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.MessageBus.Consumer
{
    public abstract class MessageConsumerBase<TDto, TServiceResponseDto> : BackgroundService
        where TServiceResponseDto : class
    {
        private readonly RabbitMQSettings _settings;
        private readonly ISerializer _serializer;
        private readonly string _queueName;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageConsumerBase(RabbitMQSettings settings, ISerializer serializer, string queueName)
        {
            _settings = settings;
            _serializer = serializer;
            _queueName = queueName;
            var factory = new ConnectionFactory
            {
                HostName = _settings.Hostname,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        //Used only from integration test
        public override Task StartAsync(CancellationToken stoppingToken)
        {
            ExecuteAsync(stoppingToken);
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                TDto objDto = _serializer.DeserializeObject<TDto>(ea.Body.ToArray());

                var correlationId = ea.BasicProperties.CorrelationId;
                string replyTo = ea.BasicProperties.ReplyTo;
                ResultDto<TServiceResponseDto> response = await HandleMessage(objDto);

                _channel.BasicAck(ea.DeliveryTag, false);

                var responseBody = _serializer.SerializeObjectToByteArray(response);
                IBasicProperties basicProperties = _channel.CreateBasicProperties();
                basicProperties.CorrelationId = correlationId;

                _channel.BasicPublish(exchange: "", routingKey: replyTo, basicProperties: basicProperties, body: responseBody);
            };

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task<ResultDto<TServiceResponseDto>> HandleMessage(TDto objDto)
        {
            try
            {
                TServiceResponseDto result = await CallService(objDto).ThrowAsync();
                return new ResultDto<TServiceResponseDto>(result);
            }
            catch (Exception e)
            {
                return new ResultDto<TServiceResponseDto>(e.Message);
            }
        }

        protected abstract Task<Result<TServiceResponseDto>> CallService(TDto objDto);

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

    }
}
