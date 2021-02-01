using RetailInMotion.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailInMotion.Data.Repository.Interfaces
{
    public interface IOrderProductsRepository
    {
        Task AddOrderProducts(Guid orderProductId, List<(Guid, int)> orderProducts);
        Task CancelProductsFromOrder(Guid orderid);
        Task<IEnumerable<OrderProductEntity>> GetByOrderId(Guid orderId);
        Task UpdateOrderProductsQuantity(List<(Guid, int)> products, Guid orderId);
        Task CommitTransaction();
    }
}
