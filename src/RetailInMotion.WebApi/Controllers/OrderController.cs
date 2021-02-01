using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RetailInMotion.Model.Dto;
using RetailInMotion.Model.Dto.Message;
using RetailInMotion.WebApi.Publisher.Orders;
using Shared.MessageBus;
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
        private readonly GetOrderMessageSender _getOrderMessageSender;
        private readonly PaginatedOrderMessageSender _paginatedOrderMessageSender;

        public OrderController(RabbitMQSettings settings, ISerializer serializer)
        {
            _orderCreationMessageSender = new OrderCreationMessageSender(settings, serializer);
            _updateOrderDeliveryMessageSender = new UpdateOrderDeliveryMessageSender(settings, serializer);
            _updateOrderProductsMessageSender = new UpdateOrderProductsMessageSender(settings, serializer);
            _cancelOrderMessageSender = new CancelOrderMessageSender(settings, serializer);
            _getOrderMessageSender = new GetOrderMessageSender(settings, serializer);
            _paginatedOrderMessageSender = new PaginatedOrderMessageSender(settings, serializer);
        }

        [HttpPost("create")]
        public async Task<ResultDto<OrderCreationMessageResponseDto>> Create(OrderCreationDto order)
        {
            Console.WriteLine("called controller");

            using (_orderCreationMessageSender)
                return await _orderCreationMessageSender.CallAsync(order);
        }

        [HttpPut("updatedelivery/{orderId}")]
        public async Task<ResultDto<UpdateOrderDeliveryMessageResponseDto>> UpdateDelivery(Guid orderId, DeliveryDto delivery)
        {
            using (_updateOrderDeliveryMessageSender)
                return await _updateOrderDeliveryMessageSender.CallAsync(new Model.OrderDelivery(orderId, delivery));
        }

        [HttpPut("updateproducts/{orderId}")]
        public async Task<ResultDto<UpdateProductsMessageResponseDto>> UpdateProducts(Guid orderId, List<ProductQuantityDto> products)
        {
            using (_updateOrderProductsMessageSender)
                return await _updateOrderProductsMessageSender.CallAsync(new Model.OrderItems(orderId, products));
        }

        [HttpDelete("{orderId}")]
        public async Task<ResultDto<CancelOrderMessageResponseDto>> Cancel(Guid orderId)
        {
            using (_cancelOrderMessageSender)
                return await _cancelOrderMessageSender.CallAsync(orderId);
        }

        [HttpGet("{orderId}")]
        public async Task<ResultDto<GetOrderMessageResponseDto>> Get(Guid orderId)
        {
            using (_getOrderMessageSender)
                return await _getOrderMessageSender.CallAsync(orderId);
        }

        [HttpGet("page/{pageNumber}")]
        public async Task<ResultDto<PaginatedOrdersMessageResponseDto>> GetPaginated(int pageNumber)
        {
            using (_paginatedOrderMessageSender) 
                return await _paginatedOrderMessageSender.CallAsync(pageNumber);
        }
    }
}
