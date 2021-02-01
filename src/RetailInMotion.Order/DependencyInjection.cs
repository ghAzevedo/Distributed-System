using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using RetailIM.Data.Repository;
using RetailInMotion.BackgroundWorkers.Consumers;
using RetailInMotion.Data.Repository;
using RetailInMotion.Data.Repository.Interfaces;
using RetailInMotion.ServiceDependencies.Orders;
using RetailInMotion.Services.Order;
using Shared.Data;
using Shared.MessageBus;
using Shared.Utils.Serialization;
using System.Data.Common;

namespace RetailInMotion.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddRabitMQ(configuration)
                .AddDbConnectors(configuration)
                .AddRepositories()
                .AddServiceDependencies()
                .AddServices()
                .AddCommon()
                .AddBackgroundWorkers();
        }

        private static IServiceCollection AddRabitMQ(this IServiceCollection di, IConfiguration configuration)
        {
            IConfigurationSection rabbitMqConfig = configuration.GetSection("RabbitMq");
            return di
                .AddTransient(x => BuildRabbitMQSettings());

            RabbitMQSettings BuildRabbitMQSettings()
            {
                return new RabbitMQSettings(rabbitMqConfig.GetValue<string>("hostname"),
                    rabbitMqConfig.GetValue<string>("user"),
                    rabbitMqConfig.GetValue<string>("password"));

            }
        }
        private static IServiceCollection AddDbConnectors(this IServiceCollection di, IConfiguration configuration)
        {
            return di
                .AddTransient<DbConnection>(x => new MySqlConnection(configuration.GetValue<string>("connectionStringMysql")))
                .AddTransient<TransactionalWrapper>();
        }

        private static IServiceCollection AddRepositories(this IServiceCollection di)
        {
            return di
                .AddTransient<IOrderRepository, OrderRepository>()
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<IOrderProductsRepository, OrderProductsRepository>()
                .AddTransient<IDeliveryRepository, DeliveryRepository>();
        }

        private static IServiceCollection AddServiceDependencies(this IServiceCollection di)
        {
            return di
                .AddTransient<ICreateOrderHandlerDependencies, CreateOrderHandlerDependencies>()
                .AddTransient<ICancelOrderHandlerDependencies, CancelOrderHandlerDependencies>()
                .AddTransient<IRetrievePaginatedOrdersHandlerDependencies, RetrievePaginatedOrdersHandlerDependencies>()
                .AddTransient<IRetrieveSingleOrderHandlerDependencies, RetrieveSingleOrderHandlerDependencies>()
                .AddTransient<IUpdateOrderDeliveryHandlerDependencies, UpdateOrderDeliveryHandlerDependencies>()
                .AddTransient<IUpdateOrderItemsHandlerDependencies, UpdateOrderItemsHandlerDependencies>();
        }

        private static IServiceCollection AddServices(this IServiceCollection di)
        {
            return di
                    .AddTransient<CreateOrderHandler>()
                    .AddTransient<CancelOrderHandler>()
                    .AddTransient<RetrieveSingleOrderHandler>()
                    .AddTransient<UpdateOrderDeliveryHandler>()
                    .AddTransient<UpdateOrderItemsHandler>()
                    .AddTransient<RetrievePaginatedOrdersHandler>();
        }

        private static IServiceCollection AddCommon(this IServiceCollection di)
        {
            return di
                   .AddTransient<ISerializer, Serializer>();
        }

        private static IServiceCollection AddBackgroundWorkers(this IServiceCollection di)
        {
            return di
                .AddHostedService<OrderCreationConsumer>()
                .AddHostedService<CancelOrderConsumer>()
                .AddHostedService<PaginatedOrdersConsumer>()
                .AddHostedService<GetOrderConsumer>()
                .AddHostedService<UpdateOrderDeliveryConsumer>()
                .AddHostedService<UpdateOrderProductsConsumer>();
        }
    }
}
