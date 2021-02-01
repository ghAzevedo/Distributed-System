using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using RetailIM.Data.Repository;
using RetailInMotion.BackgroundWorkers.Consumers;
using RetailInMotion.Data.Repository;
using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.Model.Dto;
using RetailInMotion.Model.Dto.Message;
using RetailInMotion.ServiceDependencies.Orders;
using RetailInMotion.Services.Order;
using RetailInMotion.WebApi;
using Shared.Data;
using Shared.MessageBus;
using Shared.Utils.Serialization;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RetailIM.IntegrationTest.Flow
{
    public class Test_Order_Flow
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly OrderController _controller;

        public Test_Order_Flow()
        {
            _serviceProvider = BuildServiceProvider();
            _controller = _serviceProvider.GetService<OrderController>();
        }

        [Fact]
        public async Task Test_Full_Order_Workflow()
        {
            Guid productId = new Guid("2a6b0daf-5bb7-4905-be03-3886b8b9d91a");
            
            var orderId = await Test_CreateOrder(productId);
            var delivery = await Test_UpdateOrder(orderId);
            await Test_GetOrderSingle(orderId, delivery);
            
            
            await Test_UpdateProductQuantity(productId, orderId);
            
            await Test_ListOrders();
            await Test_CancelOrder(orderId);
        }
        
        private async Task<Guid> Test_CreateOrder(Guid productId)
        {
            OrderCreationDto orderCreation = new OrderCreationDto()
            {
                Delivery = new DeliveryDto()
                {
                    City = "city",
                    Country = "country",
                    Street = "street"
                },
                Products = new List<ProductQuantityDto>()
                {
                    new ProductQuantityDto()
                    {
                        ProductId = productId, 
                        Quantity = 1 
                    }
                }
            };

            using var orderCreationConsumer = _serviceProvider.GetService<OrderCreationConsumer>();
            await orderCreationConsumer.StartAsync(CancellationToken.None);
            ResultDto<OrderCreationMessageResponseDto> result = await _controller.Create(orderCreation);
            
            Assert.NotEqual(Guid.NewGuid(), result.Result.OrderId);

            return result.Result.OrderId;
        }

        private async Task<DeliveryDto> Test_UpdateOrder(Guid orderId)
        {
            DeliveryDto updatedDelivery = new DeliveryDto()
            {
                City = "city",
                Street = "street",
                Country = "country"
            };

            using var updateOrderconsumer = _serviceProvider.GetService<UpdateOrderDeliveryConsumer>();
            await updateOrderconsumer.StartAsync(CancellationToken.None);
            var deliveryResponse = await _controller.UpdateDelivery(orderId, updatedDelivery);

            Assert.True(deliveryResponse.Result.Success);

            return updatedDelivery;
        }

        private async Task Test_UpdateProductQuantity(Guid productId, Guid orderId)
        {
            var productQuantityDtos = new List<ProductQuantityDto>
            {
                new ProductQuantityDto
                {
                    ProductId = productId, 
                    Quantity = 5
                }
            };

            using var updateOrderProductsConsumer = _serviceProvider.GetService<UpdateOrderProductsConsumer>();
            await updateOrderProductsConsumer.StartAsync(CancellationToken.None);
            var result = await _controller.UpdateProducts(orderId, productQuantityDtos);
            
            Assert.True(result.Result.Success);
        }

        private async Task Test_ListOrders()
        {
            using PaginatedOrdersConsumer paginatedOrdersConsumer = _serviceProvider.GetService<PaginatedOrdersConsumer>();

            await paginatedOrdersConsumer.StartAsync(CancellationToken.None);
            var result = await _controller.GetPaginated(1);
            var orderQuantity = result.Result.Orders.Count;

            Assert.True(orderQuantity > 0);
        }

        private async Task Test_GetOrderSingle(Guid orderId, DeliveryDto deliveryDto)
        {
            OrderDto getOrderDto;
            using (var getOrderConsumer = _serviceProvider.GetService<GetOrderConsumer>())
            {
                await getOrderConsumer.StartAsync(CancellationToken.None);
                ResultDto<GetOrderMessageResponseDto> result = await _controller.Get(orderId);
                getOrderDto = result.Result.Order;
            }

            Assert.Single(getOrderDto.Products);
            Assert.Equal(deliveryDto.City, getOrderDto.DeliveryDto.City);
            Assert.Equal(deliveryDto.Country, getOrderDto.DeliveryDto.Country);
            Assert.Equal(deliveryDto.Street, getOrderDto.DeliveryDto.Street);
        }

        private async Task Test_CancelOrder(Guid orderId)
        {
            using var deleteOrderConsumer = _serviceProvider.GetService<CancelOrderConsumer>();
            await deleteOrderConsumer.StartAsync(CancellationToken.None);
            var result = await _controller.Cancel(orderId);

            Assert.True(result.Result.Success);
        }

        private ServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton(x => BuildRabbitMQSettings())
                .AddSingleton<ISerializer, Serializer>()
                .AddScoped<DbConnection>(x
                    => new MySqlConnection("Server=127.0.0.1;Port=4306;Database=retailinmotion;Uid=root;password=password1;Allow User Variables=True;"))
                .AddScoped<TransactionalWrapper>()
                .AddScoped<IOrderRepository, OrderRepository>()
                .AddScoped<IProductRepository, ProductRepository>()
                .AddScoped<IOrderProductsRepository, OrderProductsRepository>()
                .AddScoped<IDeliveryRepository, DeliveryRepository>()
                .AddTransient<ICreateOrderHandlerDependencies, CreateOrderHandlerDependencies>()
                .AddTransient<ICancelOrderHandlerDependencies, CancelOrderHandlerDependencies>()
                .AddTransient<IRetrievePaginatedOrdersHandlerDependencies, RetrievePaginatedOrdersHandlerDependencies>()
                .AddTransient<IRetrieveSingleOrderHandlerDependencies, RetrieveSingleOrderHandlerDependencies>()
                .AddTransient<IUpdateOrderDeliveryHandlerDependencies, UpdateOrderDeliveryHandlerDependencies>()
                .AddTransient<IUpdateOrderItemsHandlerDependencies, UpdateOrderItemsHandlerDependencies>()
                .AddTransient<CreateOrderHandler>()
                .AddTransient<CancelOrderHandler>()
                .AddTransient<RetrievePaginatedOrdersHandler>()
                .AddTransient<RetrieveSingleOrderHandler>()
                .AddTransient<UpdateOrderDeliveryHandler>()
                .AddTransient<UpdateOrderItemsHandler>()
                .AddScoped<OrderController>()
                .AddScoped<OrderCreationConsumer>()
                .AddScoped<UpdateOrderDeliveryConsumer>()
                .AddScoped<UpdateOrderProductsConsumer>()
                .AddScoped<PaginatedOrdersConsumer>()
                .AddScoped<GetOrderConsumer>()
                .AddScoped<CancelOrderConsumer>()
                .BuildServiceProvider();

            RabbitMQSettings BuildRabbitMQSettings()
            {
                return new RabbitMQSettings("localhost",
                    "guest",
                    "guest");
            }
        }
    }
}
