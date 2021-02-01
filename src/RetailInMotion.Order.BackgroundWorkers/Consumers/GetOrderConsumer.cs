using RetailInMotion.Model.Dto;
using RetailInMotion.Services.Order;
using Shared.MessageBus;
using Shared.MessageBus.Consumer;
using Shared.Utils;
using Shared.Utils.Serialization;
using System;
using System.Threading.Tasks;


namespace RetailInMotion.BackgroundWorkers.Consumers
{
    public class GetOrderConsumer : MessageConsumerBase<Guid, GetOrderMessageResponseDto>
    {
        private readonly RetrieveSingleOrderHandler _handler;

        public GetOrderConsumer(RabbitMQSettings settings, ISerializer serializer, RetrieveSingleOrderHandler handler)
            : base(settings, serializer, QueueName.RetrieveOrder)
        {
            _handler = handler;
        }

        protected override async Task<Result<GetOrderMessageResponseDto>> CallService(Guid objDto)
        {
            return await _handler.Handle(objDto);
        }
    }
}