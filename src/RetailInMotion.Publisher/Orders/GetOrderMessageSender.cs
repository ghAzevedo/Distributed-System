using RetailInMotion.Model.Dto;
using Shared.MessageBus;
using Shared.Utils.Serialization;
using System;

namespace RetailInMotion.WebApi.Publisher.Orders
{
    public class GetOrderMessageSender : MessageSender<Guid, GetOrderMessageResponseDto>
    {
        public GetOrderMessageSender(RabbitMQSettings settings, ISerializer serializer)
            : base(settings, QueueName.RetrieveOrder, serializer)
        {
        }
    }
}
