using Shared.MessageBus;
using Shared.Utils.Serialization;
using System;

namespace RetailInMotion.WebApi.Publisher.Orders
{
    public class GetOrderMessageSender : MessageSender<Guid>
    {
        public GetOrderMessageSender(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.RetrieveOrder, serializer)
        {
        }
    }
}
