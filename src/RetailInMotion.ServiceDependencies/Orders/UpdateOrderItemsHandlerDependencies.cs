using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.Model.Entities;
using RetailInMotion.Services.Order;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailInMotion.ServiceDependencies.Orders
{
    public class UpdateOrderItemsHandlerDependencies : IUpdateOrderItemsHandlerDependencies
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProductsRepository _orderProductRepository;
        private readonly IProductRepository _productRepository;

        public UpdateOrderItemsHandlerDependencies(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IOrderProductsRepository orderProductRepository)
        {
            _orderRepository = orderRepository;
            _orderProductRepository = orderProductRepository;
            _productRepository = productRepository;
        }

        public async Task<ProductEntity?> GetById(Guid productid)
            => await _productRepository.GetById(productid);

        public async Task<OrderEntity?> GetOrderById(Guid orderId)
            => await _orderRepository.GetSingle(orderId);

        public async Task<IEnumerable<OrderProductEntity>> GetOrderProductsByOrderId(Guid orderId)
            => await _orderProductRepository.GetByOrderId(orderId);

        public async Task<ProductEntity?> GetProductById(Guid productId)
            => await _productRepository.GetById(productId);

        public Task UpdateOrderProductQuantity(List<(Guid, int)> products, Guid orderId)
            => _orderProductRepository.UpdateOrderProductsQuantity(products, orderId);

        public async Task UpdateProductStock(List<(Guid, int)> products)
            => await _productRepository.UpdateProductStock(products);

        public async Task Commit()
        {
            await _productRepository.CommitTransaction();
            await _orderProductRepository.CommitTransaction();
        }

    }
}
