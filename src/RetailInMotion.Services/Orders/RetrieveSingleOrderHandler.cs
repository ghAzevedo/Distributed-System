using RetailInMotion.Model.Dto;
using RetailInMotion.Model.Entities;
using RetailInMotion.Services.Mappers;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailInMotion.Services.Order
{
    public interface IRetrieveSingleOrderHandlerDependencies
    {
        Task<OrderEntity?> GetOrderById(Guid orderId);
        Task<IEnumerable<OrderProductEntity>> GetOrderProductsByOrderId(Guid orderId);
        Task<IEnumerable<ProductEntity>> GetProductsByIds(List<Guid> productIds);
        Task<DeliveryEntity> GetDeliveryById(Guid orderId);

    }

    public class RetrieveSingleOrderHandler
    {
        private readonly IRetrieveSingleOrderHandlerDependencies _dependencies;

        public RetrieveSingleOrderHandler(IRetrieveSingleOrderHandlerDependencies retrieveSingleOrderHandlerDependencies)
        {
            _dependencies = retrieveSingleOrderHandlerDependencies;
        }

        public async Task<Result<OrderDto>> Execute(Guid orderId)
        {
            return await GetOrder(orderId)
                .Combine(_ => GetOrderProducts(orderId))
                .Combine(x => GetProducts(x.Item2))
                .Combine(_ => GetDelivery(orderId))
                .MapAsync(MapToOrderDto);

            OrderDto MapToOrderDto(
                (OrderEntity order,
                IEnumerable<OrderProductEntity> orderProducts,
                IEnumerable<ProductDto> products,
                DeliveryEntity delivery) arg)
            {
                return new OrderDto(orderId, arg.order.CreationTimeUtc, arg.delivery.ToDto(), arg.products.ToList());
            }
        }

        private async Task<IEnumerable<ProductDto>> GetProducts(IEnumerable<OrderProductEntity> orderProducts)
        {
            var products = await _dependencies.GetProductsByIds(orderProducts.Select(x => x.ProductId).ToList());
            return products.Select(ProductMapper.ToDto);
        }

        private async Task<Result<OrderEntity>> GetOrder(Guid orderId)
        {
            var orderResult = await _dependencies.GetOrderById(orderId);

            return orderResult == null
               ? Result.Failure<OrderEntity>($"order with id {orderId} does not exist.")
               : orderResult;
        }

        private async Task<IEnumerable<OrderProductEntity>> GetOrderProducts(Guid orderId)
            => await _dependencies.GetOrderProductsByOrderId(orderId);

        private async Task<DeliveryEntity> GetDelivery(Guid orderId)
            => await _dependencies.GetDeliveryById(orderId);


    }
}
