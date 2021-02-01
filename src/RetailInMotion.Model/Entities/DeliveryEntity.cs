using System;

namespace RetailInMotion.Model.Entities
{
    public class DeliveryEntity
    {
        public readonly Guid? DeliveryId;
        public readonly string Country;
        public readonly string City;
        public readonly string Street;
        public readonly bool? Deleted;

        public DeliveryEntity(Guid? deliveryId, string country, string city, string street, bool? deleted)
        {
            DeliveryId = deliveryId;
            Country = country;
            City = city;
            Street = street;
            Deleted = deleted;
        }

        //dapper constructor
        public DeliveryEntity(string deliveryId, string country, string city, string street, bool? deleted)
        {
            DeliveryId = new Guid(deliveryId);
            Country = country;
            City = city;
            Street = street;
            Deleted = deleted;
        }
    }
}
