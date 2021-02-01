using RetailInMotion.Model;
using RetailInMotion.Model.Dto;
using Shared.MessageBus;
using Shared.Utils.Serialization;

namespace RetailInMotion.WebApi.Publisher.Orders
{
    public class UpdateOrderProductsMessageSender : MessageSender<OrderItems, UpdateProductsMessageResponseDto>
    {
        public UpdateOrderProductsMessageSender(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.UpdateOrderProducts, serializer)
        {
        }
    }
}
