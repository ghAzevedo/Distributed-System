using RetailInMotion.Model.Entities;
using System;
using System.Threading.Tasks;

namespace RetailInMotion.Data.Repository.Interfaces
{
    public interface IDeliveryRepository
    {
        Task AddSingle(DeliveryEntity delivery, Guid orderId);
        Task UpdateOrderAddress(Guid orderId, DeliveryEntity delivery);
        Task CancelByOrderId(Guid orderID);
        Task<DeliveryEntity> GetDeliveryById(Guid orderId);
        Task CommitTransaction();
    }
}
