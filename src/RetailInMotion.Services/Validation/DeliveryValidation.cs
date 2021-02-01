using RetailInMotion.Model.Dto;
using Shared.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RetailInMotion.Services.Validation
{
    public static class DeliveryValidation
    {
        public static Result<DeliveryDto> ValidateDelivery(this DeliveryDto delivery)
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(delivery.City))
                errors.Add("City cannot be null");

            if (string.IsNullOrEmpty(delivery.Country))
                errors.Add("City cannot be null");

            if (string.IsNullOrEmpty(delivery.Street))
                errors.Add("City cannot be null");

            return errors.Any()
                ? Result.Failure<DeliveryDto>(errors.ToImmutableArray())
                : delivery;
        }

    }
}
