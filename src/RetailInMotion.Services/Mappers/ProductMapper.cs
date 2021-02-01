using RetailInMotion.Model.Dto;
using RetailInMotion.Model.Entities;

namespace RetailInMotion.Services.Mappers
{
    public static class ProductMapper
    {
        public static ProductDto ToDto(this ProductEntity product)
        {
            return new ProductDto(product.ProductId, product.Name, product.TotalStock);
        }

    }

}
