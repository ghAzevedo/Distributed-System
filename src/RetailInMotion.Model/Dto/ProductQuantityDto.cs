using System;

namespace RetailInMotion.Model.Dto
{
    public class ProductQuantityDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductQuantityDto() { }
    }
}
