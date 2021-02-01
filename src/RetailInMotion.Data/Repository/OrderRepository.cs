using Dapper;
using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.Model.Entities;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RetailInMotion.Data.Repository
{
    public class OrderRepository : RepositoryBase, IOrderRepository
    {
        public OrderRepository(TransactionalWrapper connection) 
            : base(connection)
        {
        }

        public async Task<Guid> AddSingle()
        {
            var orderId = Guid.NewGuid();

            string sql = $"insert into {TableDescription.TableName.Order} (" +
                $"{TableDescription.Order.OrderId}, " +
                $"{TableDescription.Order.CreationTimeUtc})" +
                $"  VALUES (" +
                $"@{nameof(OrderEntity.OrderId)}, " +
                $"@{nameof(OrderEntity.CreationTimeUtc)});";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            await connection.ExecuteAsync(sql, new
            {
                orderId,
                CreationTimeUtc = DateTime.UtcNow
            });

            return orderId;
        }

        public async Task AddOrderItems(Guid orderProductId, List<(Guid, int)> orderProducts)
        {
            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            foreach (var order in orderProducts)
            {
                var sql = $"insert into {TableDescription.TableName.OrderProduct} (" +
                    $"{TableDescription.OrderProduct.OrderId}, " +
                    $"{TableDescription.OrderProduct.ProductId}, " +
                    $"{TableDescription.OrderProduct.Quantity}) " +
                    $"  VALUES( " +
                    $"@{nameof(OrderProductEntity.OrderId)}, " +
                    $"@{nameof(OrderProductEntity.ProductId)}," +
                    $"@{nameof(OrderProductEntity.Quantity)});";
                await connection.ExecuteAsync(sql, new
                {
                    OrderId = orderProductId,
                    ProductId = order.Item1,
                    Quantity = order.Item2
                });
            }
        }

        public async Task<OrderEntity?> GetSingle(Guid orderId)
        {
            string sql = $"select * from {TableDescription.TableName.Order} " +
                $"Where {TableDescription.Order.OrderId} = @{TableDescription.Order.OrderId};";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            OrderEntity? result = await connection.QueryFirstOrDefaultAsync<OrderEntity>(sql, new { OrderId = orderId });

            return result;
        }

        public async Task CancelById(Guid orderId)
        {
            string sql = $"update {TableDescription.TableName.Order} set " +
                $"{TableDescription.Order.Deleted} = true " +
                $"Where {TableDescription.Order.OrderId} = @{TableDescription.Order.OrderId};";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            _ = await connection.ExecuteAsync(sql, new { orderId = orderId });
        }

        public async Task<IEnumerable<OrderEntity>> GetPaginated(int page, int itemsPerPage)
        {
            string sql = $"select * from {TableDescription.TableName.Order} " +
                $"order by {nameof(OrderEntity.CreationTimeUtc).ToLower()} desc " +
                $"LIMIT @limit OFFSET @offset; ";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            return await connection.QueryAsync<OrderEntity>(sql, new
            {
                limit = itemsPerPage,
                offset = (page - 1) * itemsPerPage,
            });
        }

        public async Task<IEnumerable<OrderProductEntity>> GetByOrderId(Guid orderId)
        {
            string sql = $"select * from {TableDescription.TableName.OrderProduct} " +
                $"Where {TableDescription.OrderProduct.OrderId} = @{TableDescription.OrderProduct.OrderId}";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            return await connection.QueryAsync<OrderProductEntity>(sql, new { OrderId = orderId });
        }
    }
}
