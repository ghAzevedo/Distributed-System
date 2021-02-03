using RetailInMotion.Model;
using Shared.MessageBus;
using Shared.Utils.Serialization;

namespace RetailInMotion.WebApi.Publisher.Orders
{
    public class UpdateOrderDeliveryMessageSender : MessageSender<OrderDelivery>
    {
        public UpdateOrderDeliveryMessageSender(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.UpdateOrderDelivery, serializer)
        {
        }
    }
}
