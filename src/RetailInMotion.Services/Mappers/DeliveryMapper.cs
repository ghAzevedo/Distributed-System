using RetailInMotion.Model.Dto;
using RetailInMotion.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RetailInMotion.Services.Mappers
{
    public static class DeliveryMapper
    {
        public static DeliveryEntity ToEntity(this DeliveryDto delivery)
        {
            return new DeliveryEntity((Guid?)null, delivery.Country, delivery.City, delivery.Street, null);
        }

        public static DeliveryDto ToDto(this DeliveryEntity delivery)
        {
            return new DeliveryDto(delivery.Country, delivery.City, delivery.Street);
        }

    }

}
