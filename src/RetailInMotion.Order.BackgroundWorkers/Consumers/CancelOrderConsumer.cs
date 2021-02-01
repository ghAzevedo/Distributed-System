using RetailInMotion.Model.Dto;
using RetailInMotion.Services.Order;
using Shared.MessageBus;
using Shared.MessageBus.Consumer;
using Shared.Utils;
using Shared.Utils.Serialization;
using System;
using System.Threading.Tasks;


namespace RetailInMotion.BackgroundWorkers.Consumers
{
    public class CancelOrderConsumer : MessageConsumerBase<Guid, CancelOrderMessageResponseDto>
    {
        private readonly CancelOrderHandler _handler;

        public CancelOrderConsumer(RabbitMQSettings settings, ISerializer serializer, CancelOrderHandler handler)
            : base(settings, serializer, QueueName.CancelOrders)
        {
            _handler = handler;
        }

        protected override async Task<Result<CancelOrderMessageResponseDto>> CallService(Guid objDto)
        {
            return await _handler.Handle(objDto);
        }
    }
}
