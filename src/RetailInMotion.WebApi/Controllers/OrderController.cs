using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RetailInMotion.Model.Dto;
using RetailInMotion.Model.Dto.Message;
using RetailInMotion.Services.Order;
using RetailInMotion.WebApi.Publisher.Orders;
using Shared.MessageBus;
using Shared.Utils;
using Shared.Utils.Serialization;

namespace RetailInMotion.WebApi
{
    [ApiController]
    [Route("[Controller]")]
    public class OrderController : Controller
    {
        private readonly OrderCreationMessageSender _orderCreationMessageSender;
        private readonly UpdateOrderDeliveryMessageSender _updateOrderDeliveryMessageSender;
        private readonly UpdateOrderProductsMessageSender _updateOrderProductsMessageSender;
        private readonly CancelOrderMessageSender _cancelOrderMessageSender;

        private readonly RetrieveSingleOrderHandler _retrieveSingleOrderHandler;
        private readonly RetrievePaginatedOrdersHandler _retrievePaginatedOrdersHandler;

        public OrderController(
            RetrieveSingleOrderHandler retrieveSingleOrderHandler,
            RetrievePaginatedOrdersHandler retrievePaginatedOrdersHandler,
            RabbitMQSettings settings, 
            ISerializer serializer)
        {
            _orderCreationMessageSender = new OrderCreationMessageSender(settings, serializer);
            _updateOrderDeliveryMessageSender = new UpdateOrderDeliveryMessageSender(settings, serializer);
            _updateOrderProductsMessageSender = new UpdateOrderProductsMessageSender(settings, serializer);
            _cancelOrderMessageSender = new CancelOrderMessageSender(settings, serializer);

            _retrieveSingleOrderHandler = retrieveSingleOrderHandler;
            _retrievePaginatedOrdersHandler = retrievePaginatedOrdersHandler;
        }

        [HttpPost("create")]
        public ResultDto<string> Create(OrderCreationDto order)
        {
            Console.WriteLine("called controller");

            using (_orderCreationMessageSender)
                return _orderCreationMessageSender.CallAsync(order);
        }

        [HttpPut("updatedelivery/{orderId}")]
        public ResultDto<string> UpdateDelivery(Guid orderId, DeliveryDto delivery)
        {
            using (_updateOrderDeliveryMessageSender)
                return _updateOrderDeliveryMessageSender.CallAsync(new Model.OrderDelivery(orderId, delivery));
        }

        [HttpPut("updateproducts/{orderId}")]
        public ResultDto<string> UpdateProducts(Guid orderId, List<ProductQuantityDto> products)
        {
            using (_updateOrderProductsMessageSender)
                return _updateOrderProductsMessageSender.CallAsync(new Model.OrderItems(orderId, products));
        }

        [HttpDelete("{orderId}")]
        public ResultDto<string> Cancel(Guid orderId)
        {
            using (_cancelOrderMessageSender)
                return _cancelOrderMessageSender.CallAsync(orderId);
        }

        [HttpGet("{orderId}")]
        public async Task<Result<OrderDto>> Get(Guid orderId)
        {
            return await _retrieveSingleOrderHandler.Execute(orderId);
        }

        [HttpGet("page/{pageNumber}")]
        public async Task<Result<PaginatedOrdersMessageResponseDto>> GetPaginated(int pageNumber)
        {
                return await _retrievePaginatedOrdersHandler.Execute(pageNumber);
        }
    }
}
