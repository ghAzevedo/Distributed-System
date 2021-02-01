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
    public class UpdateOrderProductsConsumer : MessageConsumerBase<OrderItems, UpdateProductsMessageResponseDto>
    {
        private readonly UpdateOrderItemsHandler _handler;

        public UpdateOrderProductsConsumer(RabbitMQSettings settings, ISerializer serializer, UpdateOrderItemsHandler handler)
            : base(settings, serializer, QueueName.UpdateOrderProducts)
        {
            _handler = handler;
        }

        protected override async Task<Result<UpdateProductsMessageResponseDto>> CallService(OrderItems objDto)
        {
            return await _handler.Handle(objDto);
        }
    }
}