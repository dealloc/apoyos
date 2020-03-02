using System;
using System.Threading;
using System.Threading.Tasks;

namespace Apoyos.Servicebus.Contracts
{
    /// <summary>
    /// Handles the (de)serialization of domain events.
    /// </summary>
    public interface IDomainEventSerializer
    {
        /// <summary>
        /// Serialize a domain event asynchronously.
        /// </summary>
        /// <param name="domainEvent">The domain event to serialize.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to allow cancelling the asynchronous operation.</param>
        /// <typeparam name="TEvent">The type of the domain event to serialize.</typeparam>
        /// <returns>The serialized domain event as a byte array.</returns>
        Task<byte[]> SerializeAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : class, new();

        /// <summary>
        /// Deserialize a domain event from a byte array.
        /// </summary>
        /// <param name="serialized">The domain event in it's serialized form.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to allow cancelling the asynchronous operation.</param>
        /// <typeparam name="TEvent">The type of domain event to serialize into.</typeparam>
        /// <returns>An instance of <typeparamref name="TEvent"/> deserialized from <paramref name="serialized"/>.</returns>
        /// <exception cref="NotSupportedException">When the format used by <paramref name="serialized"/> is not supported by this serializer.</exception>
        Task<TEvent> DeserializeAsync<TEvent>(byte[] serialized, CancellationToken cancellationToken = default) where TEvent : class, new();
    }
}