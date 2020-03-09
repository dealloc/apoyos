using System.Threading.Tasks;
using Apoyos.Servicebus.Abstractions.Models;
using Apoyos.Servicebus.Configuration;
using Apoyos.Servicebus.Contracts;
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
            var metadata = new MessageMetadata<TEvent>(serviceName, domainEvent);
            var serialized = await _serializer.SerializeAsync(metadata).ConfigureAwait(false);
            
            _logger.LogInformation("Dispatching {EventName} {RequestId}", typeof(TEvent).FullName, metadata.Identifier);
            await _transport.SendMessageAsync<TEvent>(serialized).ConfigureAwait(false);
        }
    }
}