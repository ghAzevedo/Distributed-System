using RetailInMotion.Model.Dto;
using System;

namespace RetailInMotion.Model
{
    public class OrderDelivery
    {
        public Guid OrderId { get; set; }
        public DeliveryDto Delivery { get; set; }

        public OrderDelivery() { }

        public OrderDelivery(Guid orderId, DeliveryDto delivery)
        {
            OrderId = orderId;
            Delivery = delivery;
        }
    }
}
