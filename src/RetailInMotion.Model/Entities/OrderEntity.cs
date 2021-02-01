using System;

namespace RetailInMotion.Model.Entities
{
    public class OrderEntity
    {
        public readonly Guid OrderId;
        public readonly bool? Deleted;
        public readonly DateTime CreationTimeUtc;

        public OrderEntity(Guid orderid, bool? deleted, DateTime creationtimeutc)
        {
            OrderId = orderid;
            Deleted = deleted;
            CreationTimeUtc = creationtimeutc;
        }

        //Dapper constructor
        public OrderEntity(string orderid, bool? deleted, DateTime creationtimeutc)
        {
            OrderId = new Guid(orderid);
            CreationTimeUtc = creationtimeutc;
        }
    }
}
