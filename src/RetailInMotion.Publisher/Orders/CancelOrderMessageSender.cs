using Shared.Utils.Serialization;
using Shared.MessageBus;
using System;

namespace RetailInMotion.WebApi.Publisher.Orders
{
    public class CancelOrderMessageSender : MessageSender<Guid>
    {
        public CancelOrderMessageSender(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.CancelOrders, serializer)
        {
        }
    }
}
