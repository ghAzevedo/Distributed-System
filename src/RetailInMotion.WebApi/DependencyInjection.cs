using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RetailInMotion.ServiceDependencies.Orders;
using RetailInMotion.Services.Order;
using Shared.MessageBus;
using Shared.Utils.Serialization;

namespace RetailInMotion.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddRabitMQ(configuration)
                .AddUtils()
                .AddServiceDependencies()
                .AddServices();
        }

        private static IServiceCollection AddServiceDependencies(this IServiceCollection di)
        {
            return di
                .AddTransient<IRetrievePaginatedOrdersHandlerDependencies, RetrievePaginatedOrdersHandlerDependencies>()
                .AddTransient<IRetrieveSingleOrderHandlerDependencies, RetrieveSingleOrderHandlerDependencies>();
        }

        private static IServiceCollection AddServices(this IServiceCollection di)
        {
            return di
                    .AddTransient<RetrieveSingleOrderHandler>()
                    .AddTransient<RetrievePaginatedOrdersHandler>();
        }

        private static IServiceCollection AddRabitMQ(this IServiceCollection di, IConfiguration configuration)
        {
            IConfigurationSection rabbitMqConfig = configuration.GetSection("RabbitMq");

            return di
                .AddSingleton(x => BuildRabbitMQSettings());
                
            RabbitMQSettings BuildRabbitMQSettings()
            {
                return new RabbitMQSettings(rabbitMqConfig.GetValue<string>("hostname"),
                    rabbitMqConfig.GetValue<string>("user"),
                    rabbitMqConfig.GetValue<string>("password"));
            }
        }
        private static IServiceCollection AddUtils(this IServiceCollection di)
        {
            return di
                   .AddSingleton<ISerializer, Serializer>();
        }
    }

}
