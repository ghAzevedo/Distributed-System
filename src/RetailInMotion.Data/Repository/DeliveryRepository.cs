using Dapper;
using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.Model.Entities;
using Shared.Data;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace RetailInMotion.Data.Repository
{
    public class DeliveryRepository : RepositoryBase, IDeliveryRepository
    {
        public DeliveryRepository(TransactionalWrapper connection) 
            : base(connection)
        {
        }

        public async Task AddSingle(DeliveryEntity delivery, Guid orderId)
        {
            string sql = $"insert into {TableDescription.TableName.Delivery} (" +
                    $"{TableDescription.Delivery.DeliveryId}, " +
                    $"{TableDescription.Delivery.City}," +
                    $"{TableDescription.Delivery.Country}, " +
                    $"{TableDescription.Delivery.Street})" +
                    $"  VALUES (" +
                    $"@{nameof(DeliveryEntity.DeliveryId)}, " +
                    $"@{nameof(DeliveryEntity.City)}, " +
                    $"@{nameof(DeliveryEntity.Country)}, " +
                    $"@{nameof(DeliveryEntity.Street)});";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            _ = await connection.ExecuteAsync(sql, new
            {
                DeliveryId = orderId,
                City = delivery.City,
                Country = delivery.Country,
                Street = delivery.Street
            });
        }

        public async Task UpdateOrderAddress(Guid orderId, DeliveryEntity delivery)
        {
            string sql = $"Update {TableDescription.TableName.Delivery} set " +
                $"{TableDescription.Delivery.City} = @{nameof(DeliveryEntity.City)}, " +
                $"{TableDescription.Delivery.Country} = @{nameof(DeliveryEntity.Country)}, " +
                $"{TableDescription.Delivery.Street} = @{nameof(DeliveryEntity.Street)} " +
                $"Where {TableDescription.Delivery.DeliveryId} = @{nameof(DeliveryEntity.DeliveryId)};";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            _ = await connection.ExecuteAsync(sql, new
            {
                DeliveryId = orderId,
                City = delivery.City,
                Country = delivery.Country,
                Street = delivery.Street
            });
        }

        public async Task CancelByOrderId(Guid orderId)
        {
            string sql = $"update {TableDescription.TableName.Delivery} set " +
                $"{TableDescription.Delivery.Deleted} = true " +
                $"Where {TableDescription.Delivery.DeliveryId} = @{nameof(DeliveryEntity.DeliveryId)};";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            await connection.ExecuteAsync(sql, new
            {
                DeliveryId = orderId
            });
        }

        public async Task<DeliveryEntity> GetDeliveryById(Guid orderId)
        {
            string sql = $"select * from {TableDescription.TableName.Delivery} " +
                $"Where {TableDescription.Delivery.DeliveryId} = @{TableDescription.Delivery.DeliveryId}";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<DeliveryEntity>(sql, new { DeliveryId = orderId });
        }
    }
}
