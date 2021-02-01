using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.Model.Entities;
using RetailInMotion.Services.Order;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailInMotion.ServiceDependencies.Orders
{
    public class CreateOrderHandlerDependencies : ICreateOrderHandlerDependencies
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IOrderProductsRepository _orderProductsRepository;
        private readonly IProductRepository _productRepository;
        public CreateOrderHandlerDependencies(
            IOrderRepository orderRepository, 
            IDeliveryRepository deliveryRepository, 
            IOrderProductsRepository orderProductsRepository, 
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _deliveryRepository = deliveryRepository;
            _orderProductsRepository = orderProductsRepository;
            _productRepository = productRepository;
        }

        public async Task AddDelivery(DeliveryEntity delivery, Guid orderId) => 
            await _deliveryRepository.AddSingle(delivery, orderId);

        public async Task<Guid> AddOrder() => 
            await _orderRepository.AddSingle();

        public async Task AddOrderProducts(List<(Guid, int)> orderProducts, Guid orderId)
            => await _orderRepository.AddOrderItems(orderId, orderProducts);

        public Task UpdateProductStock(List<(Guid, int)> products)
            => _productRepository.UpdateProductStock(products);

        public async Task<IEnumerable<ProductEntity>> GetProductByIds(List<Guid> ids)
            => await _productRepository.GetByIds(ids);

        public async Task Commit()
        {
            await _orderRepository.CommitTransaction();
            await _orderProductsRepository.CommitTransaction();
            await _productRepository.CommitTransaction();
            await _deliveryRepository.CommitTransaction();
        }
    }
}
