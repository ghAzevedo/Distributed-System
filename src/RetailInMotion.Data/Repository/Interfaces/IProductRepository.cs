using RetailInMotion.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailInMotion.Data.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<ProductEntity?> GetById(Guid productid);
        Task<ReadOnlyCollection<ProductEntity>> GetByIds(List<Guid> productIds);
        Task UpdateProductStock(List<(Guid, int)> productsAllocation);
        //Task UpdateProductsUnallocation(List<(Guid, int)> productsAllocation);
        Task AddSingle(ProductEntity product);
        Task DeleteAllProducts();
        Task CommitTransaction();

    }
}
