using RetailInMotion.Model.Dto;
using RetailInMotion.Services.Order;
using Shared.MessageBus;
using Shared.MessageBus.Consumer;
using Shared.Utils;
using Shared.Utils.Serialization;
using System.Threading.Tasks;

namespace RetailInMotion.BackgroundWorkers.Consumers
{
    public class PaginatedOrdersConsumer : MessageConsumerBase<int, PaginatedOrdersMessageResponseDto>
    {
        private readonly RetrievePaginatedOrdersHandler _handler;

        public PaginatedOrdersConsumer(RabbitMQSettings settings, ISerializer serializer, RetrievePaginatedOrdersHandler service)
            : base(settings, serializer, QueueName.RetrievePaginatedOrder)
        {
            _handler = service;
        }

        protected override async Task<Result<PaginatedOrdersMessageResponseDto>> CallService(int page)
        {
            return await _handler.Handle(page);
        }
    }
}
