using System;
using System.Collections.Generic;
using Apoyos.Servicebus.Configuration;
using Apoyos.Servicebus.Contracts;
using Apoyos.Servicebus.Implementations.Serializers;
using Apoyos.Servicebus.Implementations.Servicebus;
using Apoyos.Servicebus.Implementations.Transport;
using Apoyos.Servicebus.RabbitMQ.Configuration;
using Apoyos.Servicebus.RabbitMQ.Contracts;
using Apoyos.Servicebus.RabbitMQ.Hosted;
using Apoyos.Servicebus.RabbitMQ.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

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
        /// <param name="configure">Configure the <see cref="RabbitMqServicebusConfiguration"/> instance (add events, queues, ...).</param>
        public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration, Action<RabbitMqServicebusConfiguration>? configure = default)
        {
            services.Configure<RabbitMqServicebusConfiguration>(configuration);
            if (configure != null)
            {
                services.Configure(configure);
            }
            
            
            // Add services.
            services.AddSingleton<IOptions<ServicebusConfiguration>>(p => p.GetRequiredService<IOptions<RabbitMqServicebusConfiguration>>());
            services.AddSingleton<IOptionsMonitor<ServicebusConfiguration>>(p => p.GetRequiredService<IOptionsMonitor<RabbitMqServicebusConfiguration>>());
            services.AddSingleton<IConnectionService, ConnectionService>();
            services.AddSingleton<IDomainEventSerializer, JsonDomainEventSerializer>();
            services.AddSingleton<IMessageReceiver, DefaultMessageReceiver>();
            services.AddSingleton<IServicebus, DefaultServicebus>();

            services.AddSingleton<HostedRabbitMqService>();
            services.AddSingleton<IMessageTransport>(p => p.GetRequiredService<HostedRabbitMqService>());
            services.AddHostedService(p => p.GetRequiredService<HostedRabbitMqService>());
        }

        /// <summary>
        /// Add a transient <see cref="IDomainEventHandler{TEvent}"/> for <typeparamref name="TEvent"/> and register a queue for it.
        /// </summary>
        /// <param name="services">The services to register on.</param>
        /// <param name="queueName">The name of the queue this event corresponds to.</param>
        /// <param name="properties">Queue parameters, see <see cref="IModel.QueueDeclare"/>.</param>
        /// <typeparam name="TEvent">The type of domain event to register.</typeparam>
        /// <typeparam name="THandler">The type of the domain event handler.</typeparam>
        public static void AddDomainEventListener<TEvent, THandler>(this IServiceCollection services, string queueName, IDictionary<string, object>? properties = default) where TEvent : class, new() where THandler : class, IDomainEventHandler<TEvent>
        {
            services.AddTransient<IDomainEventHandler<TEvent>, THandler>();

            services.Configure<RabbitMqServicebusConfiguration>(config =>
            {
                config.RabbitMQ.Queues.Add(typeof(TEvent), new QueueConfiguration
                {
                    QueueName = queueName,
                    Listen = true,
                    Properties = properties
                });
            });
        }

        /// <summary>
        /// Add a domain event without registering a listener for it (events you only want to publish, not handle).
        /// </summary>
        /// <param name="services">The services to register on.</param>
        /// <param name="queueName">The name of the queue this event corresponds to.</param>
        /// <param name="properties">Queue parameters, see <see cref="IModel.QueueDeclare"/>.</param>
        /// <typeparam name="TEvent">The type of domain event to register.</typeparam>
        public static void AddDomainEvent<TEvent>(this IServiceCollection services, string queueName, IDictionary<string, object>? properties = default) where TEvent : class, new()
        {
            services.Configure<RabbitMqServicebusConfiguration>(config =>
            {
                config.RabbitMQ.Queues.Add(typeof(TEvent), new QueueConfiguration
                {
                    QueueName = queueName,
                    Listen = false,
                    Properties = properties
                });
            });
        }
    }
}