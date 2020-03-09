using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Apoyos.Servicebus.Abstractions.Models;
using Apoyos.Servicebus.Configuration;
using Apoyos.Servicebus.Contracts;
using Apoyos.Servicebus.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Apoyos.Servicebus.Implementations.Transport
{
    /// <summary>
    /// The default implementation of the <see cref="IMessageReceiver"/> contract.
    /// </summary>
    public class DefaultMessageReceiver : IMessageReceiver
    {
        private readonly ServicebusConfiguration _configuration;
        private readonly ILogger<DefaultMessageReceiver> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDomainEventSerializer _serializer;
        private readonly MethodInfo _deserializeMethod;

        /// <summary>
        /// Create a new instance of <see cref="DefaultMessageReceiver"/>.
        /// </summary>
        public DefaultMessageReceiver(IOptions<ServicebusConfiguration> options, ILogger<DefaultMessageReceiver> logger, IServiceProvider serviceProvider, IDomainEventSerializer serializer)
        {
            _configuration = options.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _serializer = serializer;

            // Prefetch the Deserialize method so we don't have to re-fetch it every time HandleIncoming is called.
#pragma warning disable CS8601 // GetMethod might return null, but in this case can never do this (since we're using type safe nameof).
            _deserializeMethod = serializer.GetType().GetMethod(nameof(IDomainEventSerializer.DeserializeAsync));
#pragma warning restore CS8601
        }
        
        /// <inheritdoc cref="IMessageReceiver.HandleIncoming" />
        public async Task HandleIncoming(Type eventType, byte[] payload)
        {
            var metadataType = typeof(MessageMetadata<>).MakeGenericType(eventType);
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
            var handler = _serviceProvider.GetRequiredService(handlerType);

#pragma warning disable CS8600 // casting object? to Task is unsafe, but such is life with reflection.
#pragma warning disable CS8602 // Calling await on something that is possibly null (though it never is).
            var deserializedTask = (Task) _deserializeMethod.MakeGenericMethod(metadataType).Invoke(_serializer, new object?[]
            {
                payload,
                CancellationToken.None
            });
            await deserializedTask.ConfigureAwait(false);

            if (handlerType.GetMethod(nameof(IDomainEventHandler<object>.HandleAsync)) is { } handleMethod)
            {
                // TODO: we might want to optimize the dynamic code calls here a little.
                var metadata = ((dynamic) deserializedTask).Result;
                var deserialized = metadata.Event;

                var envelope = (metadata as BaseMessageMetadata);
                if (string.IsNullOrWhiteSpace(envelope?.Identifier) || envelope.CreatedOn == DateTime.UnixEpoch || deserialized == null)
                {
                    throw new PoisonedMessageException($"Cannot process incoming message for {eventType.FullName}.");
                }
                
                _logger.LogDebug("Handling incoming message {RequestId}", envelope?.Identifier);
                try
                {
                    await ((Task)handleMethod.Invoke(handler, new[]
                    {
                        deserialized
                    })).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    throw new DomainEventHandlerException($"An exception occured while executing {handlerType.FullName}.{nameof(IDomainEventHandler<object>.HandleAsync)}", exception);
                }
            }
        }
    }
}