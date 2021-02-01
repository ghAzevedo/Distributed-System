using RetailInMotion.Model;
using RetailInMotion.Model.Dto;
using RetailInMotion.Model.Entities;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailInMotion.Services.Order
{
    public interface IUpdateOrderItemsHandlerDependencies
    {
        Task<OrderEntity?> GetOrderById(Guid orderId);
        Task<IEnumerable<OrderProductEntity>> GetOrderProductsByOrderId(Guid orderId);
        Task<ProductEntity?> GetProductById(Guid productId);
        Task<ProductEntity?> GetById(Guid productid);
        Task UpdateOrderProductQuantity(List<(Guid, int)> products, Guid orderId);
        Task UpdateProductStock(List<(Guid, int)> products);
        Task Commit();
    }

    public class UpdateOrderItemsHandler
    {
        private readonly IUpdateOrderItemsHandlerDependencies _dependencies;

        public UpdateOrderItemsHandler(IUpdateOrderItemsHandlerDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public async Task<Result<UpdateProductsMessageResponseDto>> Handle(OrderItems orderItems)
        {
            return await GetOrder(orderItems.OrderId)
                .Bind(GetOrderProducts)
                .Bind(ValidateProducts)
                .Bind(x => CalculateProductStockBalance(x, orderItems))
                .Bind(UpdateStock)
                .Bind(_ => UpdateOrderProduct(orderItems))
                .Bind(CommitChanges)
                .MapAsync(success => new UpdateProductsMessageResponseDto { Success = success });
        }

        private async Task<Result<Unit>> UpdateOrderProduct(OrderItems orderItems)
        {
            await _dependencies.UpdateOrderProductQuantity(orderItems.ProductsQty
                .Select(a => (a.ProductId, a.Quantity)).ToList(), orderItems.OrderId);

            return Result.Unit;
        }

        private async Task<Result<List<OrderProductEntity>>> ValidateProducts(IEnumerable<OrderProductEntity> orderProducts)
        {
            var result = await Task.WhenAll(orderProducts.Select(x => GetProduct(x.ProductId)));
            return result
                .Traverse()
                .Map(_ => orderProducts.ToList());

            async Task<Result<ProductEntity>> GetProduct(Guid id)
            {
                var product = await _dependencies.GetProductById(id);
                
                return product ?? Result.Failure<ProductEntity>($"Product with id {id} not found");
            }
        }

        private async Task<Result<Guid>> GetOrder(Guid orderId)
        {
            var orderResult = await _dependencies.GetOrderById(orderId);

            return orderResult == null
               ? Result.Failure<Guid>($"order with id {orderId} does not exist.")
               : orderId;
        }

        private async Task<Result<IEnumerable<OrderProductEntity>>> GetOrderProducts(Guid orderId)
        {
            var result = await _dependencies.GetOrderProductsByOrderId(orderId);
            return result.Success();
        }

        private Task<Result<List<OrderProductEntity>>> CalculateProductStockBalance(List<OrderProductEntity> orderProducts, OrderItems orderItems)
        {
            var result = orderProducts
                .Select(ToNewOrderProductEntity)
                .ToList()
                .Success()
                .Async();

            return result;

            OrderProductEntity ToNewOrderProductEntity(OrderProductEntity currentOrderProduct)
            {
                var result = orderItems.ProductsQty.Where(x => x.ProductId == currentOrderProduct.ProductId)
                    .Select(x => new OrderProductEntity(
                        orderItems.OrderId, 
                        x.ProductId,
                        GetStockBalance(x.Quantity, currentOrderProduct.Quantity)))
                    .FirstOrDefault();

                return result ?? throw new InvalidOperationException($"Product id {currentOrderProduct.ProductId} not found");
            }

            int GetStockBalance(int quantityToBeChanged, int currentQuantity)
            {
                return currentQuantity - quantityToBeChanged;
            }
        }

        private async Task<Result<Unit>> UpdateStock(IEnumerable<OrderProductEntity> orderProducts)
        {
            await _dependencies.UpdateProductStock(orderProducts
                .Select(a => (a.ProductId, a.Quantity)).ToList());

            return Result.Unit;
        }

        private async Task<Result<bool>> CommitChanges(Unit _)
        {
            await _dependencies.Commit();

            return true;
        }
    }
}
