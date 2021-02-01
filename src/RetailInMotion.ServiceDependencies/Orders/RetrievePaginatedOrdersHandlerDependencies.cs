using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.Model.Entities;
using RetailInMotion.Services.Order;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RetailInMotion.ServiceDependencies.Orders
{
    public class RetrievePaginatedOrdersHandlerDependencies : IRetrievePaginatedOrdersHandlerDependencies
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IOrderProductsRepository _orderProductRepository;
        private readonly IProductRepository _productRepository;
        
        public RetrievePaginatedOrdersHandlerDependencies(
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

        public async Task<IEnumerable<OrderProductEntity>> GetOrderProductsByOrderId(Guid orderId)
            => await _orderProductRepository.GetByOrderId(orderId);

        public async Task<IEnumerable<OrderEntity>> GetOrders(int page, int itemsPerPage)
            => await _orderRepository.GetPaginated(page, itemsPerPage);

        public async Task<IEnumerable<ProductEntity>> GetProductsById(List<Guid> productIds)
            => await _productRepository.GetByIds(productIds);
    }
}
