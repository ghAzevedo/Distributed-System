using RetailInMotion.Model.Entities;

namespace RetailInMotion.Data
{
    public static class TableDescription
    {
        public static class TableName
        {
            public const string Delivery = "retailinmotion.delivery";
            public const string OrderProduct = "retailinmotion.orderproducts";
            public const string Order = "retailinmotion.order";
            public const string Product = "retailinmotion.product";
        }

        public static class Delivery
        {
            public static string DeliveryId = $"{nameof(DeliveryEntity.DeliveryId).ToLower()}";
            public static string City = $"{nameof(DeliveryEntity.City).ToLower()}";
            public static string Country = $"{nameof(DeliveryEntity.Country).ToLower()}";
            public static string Deleted = $"{nameof(DeliveryEntity.Country).ToLower()}";
            public static string Street = $"{nameof(DeliveryEntity.Street).ToLower()}";
        }

        public static class OrderProduct
        {
            public static string OrderId = $"{nameof(OrderProductEntity.OrderId).ToLower()}";
            public static string ProductId = $"{nameof(OrderProductEntity.ProductId).ToLower()}";
            public static string Quantity = $"{nameof(OrderProductEntity.Quantity).ToLower()}";
        }

        public static class Order
        {
            public static string OrderId = $"{nameof(OrderEntity.OrderId).ToLower()}";
            public static string Deleted = $"{nameof(OrderEntity.Deleted).ToLower()}";
            public static string CreationTimeUtc = $"{nameof(OrderEntity.CreationTimeUtc).ToLower()}";
        }

        public static class Product
        {
            public static string ProductId = $"{nameof(ProductEntity.ProductId).ToLower()}";
            public static string Name = $"{nameof(ProductEntity.Name).ToLower()}";
            public static string TotalStock = $"{nameof(ProductEntity.TotalStock).ToLower()}";
        }
    }
}
