using System;

namespace RetailInMotion.Model.Entities
{
    public class ProductEntity
    {
        public readonly Guid ProductId;
        public readonly string Name;
        public readonly int TotalStock;

        public ProductEntity(Guid productid, string name, int totalstock)
        {
            ProductId = productid;
            Name = name;
            TotalStock = totalstock;
        }

        //Dapper constructor
        public ProductEntity(string productid, string name, int totalstock)
        {
            ProductId = new Guid(productid);
            Name = name;
            TotalStock = totalstock;
        }
    }
}
