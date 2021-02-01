using Dapper;
using RetailInMotion.Data;
using RetailInMotion.Data.Repository;
using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.Model.Entities;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RetailIM.Data.Repository
{
    public class ProductRepository : RepositoryBase, IProductRepository
    {
        public ProductRepository(TransactionalWrapper connection) : base(connection)
        {
        }

        public async Task<ProductEntity?> GetById(Guid productid)
        {
            ReadOnlyCollection<ProductEntity> products = await GetByIds(new List<Guid>() { productid });
            return products.FirstOrDefault();
        }

        public async Task<ReadOnlyCollection<ProductEntity>> GetByIds(List<Guid> productIds)
        {
            string sql = $"select * from {TableDescription.TableName.Product} " +
                $"where {TableDescription.Product.ProductId} in @ids;";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            IEnumerable<ProductEntity> result = await connection.QueryAsync<ProductEntity>(sql, new
            {
                ids = productIds.ToArray()
            });

            return result.ToList().AsReadOnly();
        }

        public async Task UpdateProductStock(List<(Guid, int)> products)
        {
            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();

            foreach ((Guid, int) product in products)
            {
                string sql = $"update {TableDescription.TableName.Product} " +
                    $"set {TableDescription.Product.TotalStock} = {TableDescription.Product.TotalStock} + (@quantity) " +
                    $"where {TableDescription.Product.ProductId} = @{TableDescription.Product.ProductId};";
                _ = await connection.ExecuteAsync(sql, new
                {
                    quantity = product.Item2,
                    ProductId = product.Item1
                });
            }
        }

        public async Task AddSingle(ProductEntity product)
        {
            string sql = $"insert into {TableDescription.TableName.Product} " +
                $" ({TableDescription.Product.ProductId}, " +
                $"{TableDescription.Product.Name}," +
                $"{TableDescription.Product.TotalStock}) " +
                $"  VALUES (" +
                $"@{nameof(ProductEntity.ProductId)}, " +
                $"@{nameof(ProductEntity.Name)}," +
                $"@{nameof(ProductEntity.TotalStock)}) ;";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            await connection.QueryAsync<ProductEntity>(sql, new
            {
                ProductId = product.ProductId,
                Name = product.Name,
                TotalStock = product.TotalStock
            });
        }

        public async Task DeleteAllProducts()
        {
            string sql = $"delete from {TableDescription.TableName.Product};";

            DbConnection connection = await _connectionnWrapper.GetConnectionAsync();
            await connection.ExecuteAsync(sql);
        }
    }
}
