using System;
using System.Linq;
using System.Threading.Tasks;
using Apoyos.Servicebus.Abstractions.Models;
using Apoyos.Servicebus.Configuration;
using Apoyos.Servicebus.Contracts;
using Apoyos.Servicebus.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Apoyos.Servicebus.Implementations.Servicebus
{
    /// <summary>
    /// Default implementation of <see cref="IServicebus"/>.
    /// </summary>
    public class DefaultServicebus : IServicebus
    {
        private readonly ILogger<DefaultServicebus> _logger;
        private readonly IOptionsMonitor<ServicebusConfiguration> _config;
        private readonly IDomainEventSerializer _serializer;
        private readonly IMessageTransport _transport;

        /// <summary>
        /// Create a new instance of <see cref="DefaultServicebus"/>.
        /// </summary>
        public DefaultServicebus(ILogger<DefaultServicebus> logger, IOptionsMonitor<ServicebusConfiguration> config, IDomainEventSerializer serializer, IMessageTransport transport)
        {
            _logger = logger;
            _config = config;
            _serializer = serializer;
            _transport = transport;
        }
        
        /// <inheritdoc cref="IServicebus.PublishAsync{TEvent}" />
        public async Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : class, new()
        {
            var serviceName = _config.CurrentValue.ServiceName;
            var eventName = GetEventName<TEvent>();
            var metadata = new MessageMetadata<TEvent>(serviceName, domainEvent);
            var serialized = await _serializer.SerializeAsync(metadata).ConfigureAwait(false);
            
            _logger.LogInformation("Dispatching {EventName} {RequestId}", eventName, metadata.Identifier);
            await _transport.SendMessageAsync(eventName, serialized).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the configured name of <typeparamref name="TEvent"/>.
        /// </summary>
        /// <returns>The application name for <typeparamref name="TEvent"/> as configured in <see cref="ServicebusConfiguration"/>.</returns>
        /// <exception cref="Exception">When <typeparamref name="TEvent"/> is not (correctly) configured.</exception>
        private string GetEventName<TEvent>()
        {
            var name = _config.CurrentValue.Events
                .Where(p => p.Value == typeof(TEvent))
                .Select(p => p.Key)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new UnknownEventException($"There's no name configured for event type {typeof(TEvent).FullName}.");
            }

            return name;
        }
    }
}