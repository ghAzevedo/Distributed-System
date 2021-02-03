using RetailInMotion.Model;
using Shared.MessageBus;
using Shared.Utils.Serialization;

namespace RetailInMotion.WebApi.Publisher.Orders
{
    public class UpdateOrderProductsMessageSender : MessageSender<OrderItems>
    {
        public UpdateOrderProductsMessageSender(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.UpdateOrderProducts, serializer)
        {
        }
    }
}
