using Apoyos.Servicebus.Configuration;
using Apoyos.Servicebus.Contracts;
using Apoyos.Servicebus.Implementations.Serializers;
using Apoyos.Servicebus.Implementations.Transport;
using Apoyos.Servicebus.RabbitMQ.Configuration;
using Apoyos.Servicebus.RabbitMQ.Contracts;
using Apoyos.Servicebus.RabbitMQ.Hosted;
using Apoyos.Servicebus.RabbitMQ.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Apoyos.Servicebus.RabbitMQ.Extensions
{
    /// <summary>
    /// Contains extension methods for the <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add RabbitMQ as the transport layer for <see cref="IServicebus"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
        /// <param name="configuration">The configuration section to bind.</param>
        public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration != null)
            {
                services.Configure<RabbitMqServicebusConfiguration>(configuration);
            }
            
            
            // Add services.
            services.AddSingleton<IOptions<ServicebusConfiguration>>(p => p.GetRequiredService<IOptions<RabbitMqServicebusConfiguration>>());
            services.AddSingleton<IOptionsMonitor<ServicebusConfiguration>>(p => p.GetRequiredService<IOptionsMonitor<RabbitMqServicebusConfiguration>>());
            services.AddSingleton<IConnectionService, ConnectionService>();
            services.AddSingleton<IDomainEventSerializer, JsonDomainEventSerializer>();
            services.AddSingleton<IMessageReceiver, DefaultMessageReceiver>();
            // services.AddSingleton<IServicebus, DefaultServicebus>();
            services.AddHostedService<HostedRabbitMqService>();
        }
    }
}