using Shared.MessageBus;
using Shared.Utils.Serialization;

namespace RetailInMotion.WebApi.Publisher.Orders
{
    public class PaginatedOrderMessageSender : MessageSender<int>
    {
        public PaginatedOrderMessageSender(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.RetrievePaginatedOrder, serializer)
        {
        }
    }
}

