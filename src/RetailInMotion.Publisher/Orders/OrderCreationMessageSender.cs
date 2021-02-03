using RetailInMotion.Model.Dto;
using Shared.MessageBus;
using Shared.Utils.Serialization;

namespace RetailInMotion.WebApi.Publisher.Orders
{
    public class OrderCreationMessageSender : MessageSender<OrderCreationDto>
    {
        public OrderCreationMessageSender(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.CreateOrder, serializer)
        {
        }
    }
}
