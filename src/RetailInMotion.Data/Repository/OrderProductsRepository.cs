using Dapper;
using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.Model.Entities;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace RetailInMotion.Data.Repository
{
    public class OrderProductsRepository : RepositoryBase, IOrderProductsRepository
    {
        public OrderProductsRepository(TransactionalWrapper connection) 
            : base(connection)
        {
        }

        public async Task AddOrderProducts(Guid orderProductId, List<(Guid, int)> orderProducts)
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
                    $"@{nameof(OrderProductEntity.ProductId)}, " +
                    $"@{nameof(OrderProductEntity.Quantity)});";
                
                await connection.ExecuteAsync(sql, new
                {
                    OrderId = orderProductId,
                    ProductId = order.Item1,
                    Quantity = order.Item2
                });
            }
        }

        public async Task CancelProductsFromOrder(Guid orderid)
        {
            string sql = $"delete from {TableDescription.TableName.OrderProduct} " +
                $"Where {TableDescription.OrderProduct.OrderId} = @{nameof(OrderProductEntity.OrderId)};";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            await connection.ExecuteAsync(sql, new
            {
                OrderId = orderid
            });
        }

        public async Task<IEnumerable<OrderProductEntity>> GetByOrderId(Guid orderId)
        {
            string sql = $"select * from {TableDescription.TableName.OrderProduct} " +
                $"Where {TableDescription.OrderProduct.OrderId} = @{TableDescription.OrderProduct.OrderId}";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            var result = await connection.QueryAsync<OrderProductEntity>(sql, new { OrderId = orderId });

            return result;
        }

        public async Task UpdateOrderProductsQuantity(List<(Guid, int)> products, Guid orderId)
        {
            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();

            foreach ((Guid, int) product in products)
            {
                string sql = $"update {TableDescription.TableName.OrderProduct} " +
                    $"set {TableDescription.OrderProduct.Quantity} = @{TableDescription.OrderProduct.Quantity} " +
                    $"where {TableDescription.OrderProduct.ProductId} = @{TableDescription.OrderProduct.ProductId} " +
                    $"  and {TableDescription.OrderProduct.OrderId} = @{TableDescription.OrderProduct.OrderId};";
                
                _ = await connection.ExecuteAsync(sql, new
                {
                    quantity = product.Item2,
                    ProductId = product.Item1,
                    orderId = orderId
                });
            }
        }

    }
}