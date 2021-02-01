using RetailInMotion.Model.Dto;
using Shared.MessageBus;
using Shared.Utils.Serialization;

namespace RetailInMotion.WebApi.Publisher.Orders
{
    public class PaginatedOrderMessageSender : MessageSender<int, PaginatedOrdersMessageResponseDto>
    {
        public PaginatedOrderMessageSender(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.RetrievePaginatedOrder, serializer)
        {
        }
    }
}

