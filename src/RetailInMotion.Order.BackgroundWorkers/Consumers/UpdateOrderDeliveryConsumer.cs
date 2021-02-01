using RetailInMotion.Model;
using RetailInMotion.Model.Dto;
using RetailInMotion.Services.Order;
using Shared.MessageBus;
using Shared.MessageBus.Consumer;
using Shared.Utils;
using Shared.Utils.Serialization;
using System.Threading.Tasks;


namespace RetailInMotion.BackgroundWorkers.Consumers
{
    public class UpdateOrderDeliveryConsumer : MessageConsumerBase<OrderDelivery, UpdateOrderDeliveryMessageResponseDto>
    {
        private readonly UpdateOrderDeliveryHandler _handler;

        public UpdateOrderDeliveryConsumer(RabbitMQSettings settings, ISerializer serializer, UpdateOrderDeliveryHandler service)
            : base(settings, serializer, QueueName.UpdateOrderDelivery)
        {
            _handler = service;
        }

        protected override async Task<Result<UpdateOrderDeliveryMessageResponseDto>> CallService(OrderDelivery objDto)
        {
            return await _handler.Handle(objDto);
        }
    }
}