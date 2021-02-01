using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.Model.Entities;
using RetailInMotion.Services.Order;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailInMotion.ServiceDependencies.Orders
{
    public class CancelOrderHandlerDependencies : ICancelOrderHandlerDependencies
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProductsRepository _orderProductRepository;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IProductRepository _productRepository;

        public CancelOrderHandlerDependencies(
            IOrderRepository orderRepository,
            IDeliveryRepository deliveryRepository,
            IProductRepository productRepository,
            IOrderProductsRepository orderProductRepository)
        {
            _orderRepository = orderRepository;
            _deliveryRepository = deliveryRepository;
            _orderProductRepository = orderProductRepository;
            _productRepository = productRepository;
        }

        public async Task<OrderEntity?> GetOrderById(Guid orderId)
            => await _orderRepository.GetSingle(orderId);

        public async Task CancelDeliveryById(Guid orderId)
            => await _deliveryRepository.CancelByOrderId(orderId);

        public async Task CancelOrderById(Guid orderId)
            => await _orderRepository.CancelById(orderId);

        public async Task<IEnumerable<OrderProductEntity>> GetOrderProductsByOrderId(Guid orderId)
            => await _orderProductRepository.GetByOrderId(orderId);

        public async Task<ProductEntity?> GetById(Guid productid)
            => await _productRepository.GetById(productid);

        public async Task UpdateProductStock(List<(Guid, int)> products)
            => await _productRepository.UpdateProductStock(products);

        public async Task Commit()
        {
            await _orderRepository.CommitTransaction();
            await _deliveryRepository.CommitTransaction();
            await _productRepository.CommitTransaction();
        }
    }
}
