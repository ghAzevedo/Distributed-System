using RetailInMotion.Model.Dto;
using RetailInMotion.Model.Entities;
using RetailInMotion.Services.Mappers;
using RetailInMotion.Services.Validation;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace RetailInMotion.Services.Order
{
    public interface ICreateOrderHandlerDependencies
    {
        Task<Guid> AddOrder();
        Task AddDelivery(DeliveryEntity delivery, Guid orderId);
        Task AddOrderProducts(List<(Guid, int)> products, Guid orderId);
        Task<IEnumerable<ProductEntity>> GetProductByIds(List<Guid> ids);
        Task UpdateProductStock(List<(Guid, int)> products);
        Task Commit();
    }

    public class CreateOrderHandler
    {
        private readonly ICreateOrderHandlerDependencies _dependencies;
        public CreateOrderHandler(ICreateOrderHandlerDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public async Task<Result<OrderCreationMessageResponseDto>> Handle(OrderCreationDto orderCreationDto)
        {
            return await ValidateOrder(orderCreationDto)
                .Bind(PersisOrder)
                .Bind(PersistDelivery)
                .Bind(PersisOrderProducts)
                .Bind(UpdateStock)
                .Bind(CommitChanges)
                .MapAsync(orderId => new OrderCreationMessageResponseDto { OrderId = orderId });
        }

        private async Task<Result<(OrderCreationDto, Guid)>> PersisOrder(OrderCreationDto order)
        {
            var orderId = await _dependencies.AddOrder();

            return (order, orderId);
        }

        private async Task<Result<(OrderCreationDto orderDto, Guid orderId)>> PersistDelivery((OrderCreationDto orderDto, Guid orderId) order)
        {
            await _dependencies.AddDelivery(order.orderDto.Delivery.ToEntity(), order.orderId);
            return order;
        }

        private async Task<Result<(OrderCreationDto orderDto, Guid orderId)>> PersisOrderProducts((OrderCreationDto, Guid) order)
        {
            await _dependencies.AddOrderProducts(
                order.Item1.Products.Select(x => (x.ProductId, x.Quantity)).ToList(), order.Item2);

            return order;
        }

        private async Task<Result<Guid>> UpdateStock((OrderCreationDto order, Guid orderId) order)
        {
            return await _dependencies.UpdateProductStock(order.order.Products
                .Select(a => (a.ProductId, (a.Quantity * -1))).ToList())
                .Success()
                .Map(_ => order.orderId)
                .Async();
        }

        private Task<Result<OrderCreationDto>> ValidateOrder(OrderCreationDto order)
        {
            return order.Delivery.ValidateDelivery()
                .Map(_ => order)
                .Async()
                .Bind(ValidateProductStock)
                .MapAsync(_ => order);
        }

        private async Task<Result<Unit>> ValidateProductStock(OrderCreationDto order)
        {
            var productsResult = await _dependencies.GetProductByIds(order.Products.Select(a => a.ProductId).ToList());

            return order.Products
                .Select(ValidateProduct)
                .Traverse()
                .Ignore();

            Result<ProductQuantityDto> ValidateProduct(ProductQuantityDto product)
            {
                ProductEntity productSearch = productsResult.FirstOrDefault(a => a.ProductId == product.ProductId);

                if (productSearch == null)
                    return Result.Failure<ProductQuantityDto>($"item {product.ProductId} could not be found.");

                if (productSearch.TotalStock < product.Quantity)
                    return Result.Failure<ProductQuantityDto>($"There is not enough stock for the item {product.ProductId}");

                return product;
            }
        }

        private async Task<Result<Guid>> CommitChanges(Guid orderId)
        {
            await _dependencies.Commit();
            return orderId;
        }
    }
}
