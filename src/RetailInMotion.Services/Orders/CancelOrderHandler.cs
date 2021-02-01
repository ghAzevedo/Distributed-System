using RetailInMotion.Model.Dto;
using RetailInMotion.Model.Entities;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailInMotion.Services.Order
{
    public interface ICancelOrderHandlerDependencies
    {
        Task<OrderEntity?> GetOrderById(Guid orderId);
        Task<IEnumerable<OrderProductEntity>> GetOrderProductsByOrderId(Guid orderId);
        Task CancelOrderById(Guid orderId);
        Task CancelDeliveryById(Guid orderId);
        Task UpdateProductStock(List<(Guid, int)> products);
        Task Commit();
    }

    public class CancelOrderHandler
    {
        private readonly ICancelOrderHandlerDependencies _dependencies;

        public CancelOrderHandler(ICancelOrderHandlerDependencies cancelOrderHandlerDependencies)
        {
            _dependencies = cancelOrderHandlerDependencies;
        }

        public async Task<Result<CancelOrderMessageResponseDto>> Handle(Guid orderId)
        {
            return await GetOrder(orderId)
                .Bind(GetOrderProducts)
                .Bind(UpdateStock)
                .Bind(_ => CancelOrder(orderId))
                .Bind(CancelDelivery)
                .Bind(CommitChanges)
                .MapAsync(_ => new CancelOrderMessageResponseDto { Success = true });
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

        private async Task<Result<Guid>> CancelOrder(Guid orderId)
        {
            await _dependencies.CancelOrderById(orderId);
            return orderId;
        }

        private async Task<Result<Unit>> CancelDelivery(Guid orderId)
        {
            await _dependencies.CancelDeliveryById(orderId);
            return Result.Unit;
        }

        private async Task<Result<Unit>> UpdateStock(IEnumerable<OrderProductEntity> orderProducts)
        {
            await _dependencies.UpdateProductStock(orderProducts
                .Select(a => (a.ProductId, a.Quantity)).ToList());

            return Result.Unit;
        }

        private async Task<Result<Unit>> CommitChanges(Unit _)
        {
            await _dependencies.Commit();
            return Result.Unit;
        }

    }
}
