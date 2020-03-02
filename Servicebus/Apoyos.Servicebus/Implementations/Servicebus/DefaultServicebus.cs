using System.Threading.Tasks;
using Apoyos.Servicebus.Abstractions.Models;
using Apoyos.Servicebus.Contracts;
using Microsoft.Extensions.Logging;

namespace Apoyos.Servicebus.Implementations.Servicebus
{
    /// <summary>
    /// Default implementation of <see cref="IServicebus"/>.
    /// </summary>
    public class DefaultServicebus : IServicebus
    {
        private readonly ILogger _logger;
        private readonly IDomainEventSerializer _serializer;
        private readonly IMessageTransport _transport;

        /// <summary>
        /// Create a new instance of <see cref="DefaultServicebus"/>.
        /// </summary>
        public DefaultServicebus(ILogger logger, IDomainEventSerializer serializer, IMessageTransport transport)
        {
            _logger = logger;
            _serializer = serializer;
            _transport = transport;
        }
        
        /// <inheritdoc cref="IServicebus.PublishAsync{TEvent}" />
        public async Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : class, new()
        {
            const string serviceName = "TODO"; // We should retrieve this from configuration.
            var metadata = new MessageMetadata<TEvent>(serviceName, domainEvent);
            var serialized = await _serializer.SerializeAsync(metadata).ConfigureAwait(false);
            
            _logger.LogInformation("Dispatch {RequestId}", metadata.Identifier);
            await _transport.SendMessageAsync(serviceName, serialized).ConfigureAwait(false);
        }
    }
}