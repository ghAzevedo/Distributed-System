using RetailInMotion.Model;
using RetailInMotion.Model.Dto;
using Shared.MessageBus;
using Shared.Utils.Serialization;

namespace RetailInMotion.WebApi.Publisher.Orders
{
    public class UpdateOrderDeliveryMessageSender : MessageSender<OrderDelivery, UpdateOrderDeliveryMessageResponseDto>
    {
        public UpdateOrderDeliveryMessageSender(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.UpdateOrderDelivery, serializer)
        {
        }
    }
}
