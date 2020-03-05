using System;
using Apoyos.Servicebus.Contracts;
using Apoyos.Servicebus.Implementations.Serializers;
using Apoyos.Servicebus.Implementations.Servicebus;
using Apoyos.Servicebus.Implementations.Transport;
using Apoyos.Servicebus.RabbitMQ.Configuration;
using Apoyos.Servicebus.RabbitMQ.Hosted;
using Apoyos.Servicebus.RabbitMQ.Services;
using Microsoft.Extensions.DependencyInjection;

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
        /// <param name="config">The configure method for configuring the transport layer and servicebus.</param>
        public static void AddRabbitMQ(this IServiceCollection services, Action<RabbitMqConfiguration> config)
        {
            services.Configure(config);
            
            // Add services.
            services.AddSingleton<ConnectionService>();
            services.AddSingleton<IDomainEventSerializer, JsonDomainEventSerializer>();
            services.AddSingleton<IMessageReceiver, DefaultMessageReceiver>();
            // services.AddSingleton<IServicebus, DefaultServicebus>();
            services.AddHostedService<HostedRabbitMqService>();
        }
    }
}