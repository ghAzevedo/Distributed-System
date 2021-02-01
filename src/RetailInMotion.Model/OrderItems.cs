using RetailInMotion.Model.Dto;
using System;
using System.Collections.Generic;

namespace RetailInMotion.Model
{
    public class OrderItems
    {
        public Guid OrderId { get; set; }
        public List<ProductQuantityDto> ProductsQty { get; set; }

        public OrderItems() { }

        public OrderItems(Guid orderId, List<ProductQuantityDto> productsQty)
        {
            OrderId = orderId;
            ProductsQty = productsQty;
        }
    }
}
