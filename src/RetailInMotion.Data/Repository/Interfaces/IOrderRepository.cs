using RetailInMotion.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailInMotion.Data.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<Guid> AddSingle();
        Task AddOrderItems(Guid orderProductId, List<(Guid, int)> orderProducts);
        Task<OrderEntity?> GetSingle(Guid orderId);
        Task CancelById(Guid orderId);
        Task<IEnumerable<OrderEntity>> GetPaginated(int page, int itemsPerPage);
        Task CommitTransaction();

    }
}
