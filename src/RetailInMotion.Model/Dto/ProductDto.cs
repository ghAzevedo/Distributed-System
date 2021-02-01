using System;

namespace RetailInMotion.Model.Dto
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        public ProductDto() { }

        public ProductDto(Guid productId, string name, int quantity)
        {
            ProductId = productId;
            Name = name;
            Quantity = quantity;
        }
    }
}
