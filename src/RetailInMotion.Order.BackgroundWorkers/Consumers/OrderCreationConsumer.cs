using RetailInMotion.Model.Dto;
using RetailInMotion.Services.Order;
using Shared.MessageBus;
using Shared.MessageBus.Consumer;
using Shared.Utils;
using Shared.Utils.Serialization;
using System.Threading.Tasks;

namespace RetailInMotion.BackgroundWorkers.Consumers
{
    public class OrderCreationConsumer : MessageConsumerBase<OrderCreationDto, OrderCreationMessageResponseDto>
    {
        private readonly CreateOrderHandler _handler;

        public OrderCreationConsumer(RabbitMQSettings settings, ISerializer serializer, CreateOrderHandler handler)
            : base(settings, serializer, QueueName.CreateOrder)
        {
            _handler = handler;
        }

        protected override async Task<Result<OrderCreationMessageResponseDto>> CallService(OrderCreationDto objDto)
        {
            return await _handler.Handle(objDto);
        }
    }
}
