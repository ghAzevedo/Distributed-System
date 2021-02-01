using System;
using System.Collections.Generic;

namespace RetailInMotion.Model.Dto
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public DateTime CreationDateUtc { get; set; }
        public DeliveryDto DeliveryDto { get; set; }
        public List<ProductDto> Products { get; set; }
        public OrderDto() { }

        public OrderDto(Guid orderId, DateTime creationDateUtc, DeliveryDto deliveryDto, List<ProductDto> products)
        {
            OrderId = orderId;
            CreationDateUtc = creationDateUtc;
            DeliveryDto = deliveryDto;
            Products = products;
        }
    }
}
