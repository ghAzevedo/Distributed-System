using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.Model.Entities;
using RetailInMotion.Services.Order;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailInMotion.ServiceDependencies.Orders
{
    public class RetrieveSingleOrderHandlerDependencies : IRetrieveSingleOrderHandlerDependencies
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IOrderProductsRepository _orderProductRepository;
        private readonly IProductRepository _productRepository;

        public RetrieveSingleOrderHandlerDependencies(
            IOrderRepository orderRepository, 
            IDeliveryRepository deliveryRepository, 
            IOrderProductsRepository orderProductRepository, 
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _deliveryRepository = deliveryRepository;
            _orderProductRepository = orderProductRepository;
            _productRepository = productRepository;
        }

        public async Task<DeliveryEntity> GetDeliveryById(Guid orderId)
            => await _deliveryRepository.GetDeliveryById(orderId);

        public async Task<OrderEntity?> GetOrderById(Guid orderId)
            => await _orderRepository.GetSingle(orderId);

        public async Task<IEnumerable<OrderProductEntity>> GetOrderProductsByOrderId(Guid orderId)
            => await _orderProductRepository.GetByOrderId(orderId);

        public async Task<IEnumerable<ProductEntity>> GetProductsByIds(List<Guid> productIds)
            => await _productRepository.GetByIds(productIds);

    }
}
