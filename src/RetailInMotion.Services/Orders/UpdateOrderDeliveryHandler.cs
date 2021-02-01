using RetailInMotion.Model;
using RetailInMotion.Model.Dto;
using RetailInMotion.Model.Entities;
using RetailInMotion.Services.Mappers;
using RetailInMotion.Services.Validation;
using Shared.Utils;
using System;
using System.Threading.Tasks;

namespace RetailInMotion.Services.Order
{
    public interface IUpdateOrderDeliveryHandlerDependencies
    {
        Task<OrderEntity?> GetOrderById(Guid orderId);
        Task<DeliveryEntity> GetDeliveryById(Guid orderId);
        Task UpdateOrderAddress(Guid orderId, DeliveryEntity delivery);
        Task Commit();
    }

    public class UpdateOrderDeliveryHandler
    {
        private readonly IUpdateOrderDeliveryHandlerDependencies _dependencies;

        public UpdateOrderDeliveryHandler(IUpdateOrderDeliveryHandlerDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public async Task<Result<UpdateOrderDeliveryMessageResponseDto>> Handle(OrderDelivery orderDelivery)
        {
            return await GetOrder(orderDelivery.OrderId)
                .Bind(_ => ValidateAddress(orderDelivery))
                .Bind(_ => UpdateOrderAddress(orderDelivery))
                .Bind(CommitChanges)
                .MapAsync(success => new UpdateOrderDeliveryMessageResponseDto { Success = success });
        }

        private Task<Result<Unit>> ValidateAddress(OrderDelivery orderDelivery)
        {
            return orderDelivery.Delivery.ValidateDelivery()
                .Async()
                .Ignore();
        }

        private async Task<Result<OrderEntity>> GetOrder(Guid orderId)
        {
            var orderResult = await _dependencies.GetOrderById(orderId);

            return orderResult ?? Result.Failure<OrderEntity>($"order with id {orderId} does not exist.");
        }

        private async Task<Result<Unit>> UpdateOrderAddress(OrderDelivery orderDelivery)
        {
            await _dependencies.UpdateOrderAddress(orderDelivery.OrderId, orderDelivery.Delivery.ToEntity());
            return Result.Unit;
        }

        private async Task<Result<bool>> CommitChanges(Unit _)
        {
            await _dependencies.Commit();

            return true;
        }
    }
}
