#pragma warning disable CA2000 // Dispose objects before losing scope. Justification: False positive, see https://github.com/dotnet/roslyn-analyzers/issues/3042
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Apoyos.Servicebus.Contracts;

namespace Apoyos.Servicebus.Implementations.Serializers
{
    /// <summary>
    /// Handles (de)serializing domain events using <see cref="JsonSerializer"/>
    /// </summary>
    public class JsonDomainEventSerializer : IDomainEventSerializer
    {
        /// <inheritdoc cref="IDomainEventSerializer.SerializeAsync{TEvent}" />
        public async Task<byte[]> SerializeAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : class, new()
        {
            await using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, domainEvent, options: null, cancellationToken).ConfigureAwait(false);

            return stream.ToArray();
        }

        /// <inheritdoc cref="IDomainEventSerializer.DeserializeAsync{TEvent}" />
        public async Task<TEvent> DeserializeAsync<TEvent>(byte[] serialized, CancellationToken cancellationToken = default) where TEvent : class, new()
        {
            try
            {
                await using var stream = new MemoryStream(serialized);
                return await JsonSerializer.DeserializeAsync<TEvent>(stream, options: null, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (JsonException exception)
            {
                throw new InvalidOperationException($"{nameof(JsonDomainEventSerializer)} does not support the given message format.", exception);
            }
        }
    }
}