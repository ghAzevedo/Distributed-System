using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.Model.Entities;
using RetailInMotion.Services.Order;
using System;
using System.Threading.Tasks;

namespace RetailInMotion.ServiceDependencies.Orders
{
    public class UpdateOrderDeliveryHandlerDependencies : IUpdateOrderDeliveryHandlerDependencies
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDeliveryRepository _deliveryRepository;
        public UpdateOrderDeliveryHandlerDependencies(
            IOrderRepository orderRepository,
            IDeliveryRepository deliveryRepository)
        {
            _orderRepository = orderRepository;
            _deliveryRepository = deliveryRepository;
        }

        public async Task<DeliveryEntity> GetDeliveryById(Guid orderId)
            => await _deliveryRepository.GetDeliveryById(orderId);

        public async Task<OrderEntity?> GetOrderById(Guid orderId)
            => await _orderRepository.GetSingle(orderId);

        public async Task UpdateOrderAddress(Guid orderId, DeliveryEntity delivery)
            => await _deliveryRepository.UpdateOrderAddress(orderId, delivery);

        public async Task Commit()
        {
            await _orderRepository.CommitTransaction();
            await _deliveryRepository.CommitTransaction();
        }
    }
}
