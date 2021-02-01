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
    public interface IRetrievePaginatedOrdersHandlerDependencies
    {
        Task<IEnumerable<OrderEntity>> GetOrders(int page, int itemsPerPage);
        Task<DeliveryEntity> GetDeliveryById(Guid orderId);
        Task<IEnumerable<OrderProductEntity>> GetOrderProductsByOrderId(Guid orderId);
        Task<IEnumerable<ProductEntity>> GetProductsById(List<Guid> productIds);
    }

    public class RetrievePaginatedOrdersHandler
    {
        private readonly IRetrievePaginatedOrdersHandlerDependencies _dependencies;

        public RetrievePaginatedOrdersHandler(IRetrievePaginatedOrdersHandlerDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public async Task<Result<PaginatedOrdersMessageResponseDto>> Handle(int page)
        {
            var orders = await GetPaginatedOrders(page, 10);

            return await GetOrderInfo(orders)
                .MapAsync(x => new PaginatedOrdersMessageResponseDto { Orders = x.ToList() });
        }

        private async Task<IEnumerable<OrderEntity>> GetPaginatedOrders(int page, int itemsPerPage)
            => await _dependencies.GetOrders(page, itemsPerPage);

        private async Task<Result<IEnumerable<OrderDto>>> GetOrderInfo(IEnumerable<OrderEntity> orders)
        {
            return await Task.WhenAll(orders.Select(BuildOrderDto));
        }

        private async Task<OrderDto> BuildOrderDto(OrderEntity orderEntity)
        {
            var deliveryDto = await GetDelivery(orderEntity.OrderId);
            var orderProductIds = (await GetOrderProductByOrderId(orderEntity.OrderId))
                .Select(x => x.ProductId)
                .ToList();

            if (orderProductIds.Count == 0)
                throw new InvalidOperationException("Order Products cannot be null.");

            var products = (await GetProducts(orderProductIds)).ToList();

            return new OrderDto(orderEntity.OrderId, orderEntity.CreationTimeUtc, deliveryDto, products);
        }

        private async Task<IEnumerable<OrderProductEntity>> GetOrderProductByOrderId(Guid orderId)
            => await _dependencies.GetOrderProductsByOrderId(orderId);

        private async Task<DeliveryDto?> GetDelivery(Guid orderId)
            => (await _dependencies.GetDeliveryById(orderId))?.ToDto();

        private async Task<IEnumerable<ProductDto>> GetProducts(List<Guid> productIds)
            => (await _dependencies.GetProductsById(productIds)).Select(x => x.ToDto());
    }
}
