using System;
using System.Threading.Tasks;
using Apoyos.Servicebus.Exceptions;

namespace Apoyos.Servicebus.Contracts
{
    /// <summary>
    /// Handles incoming messages received by the underlying transport layer (IBM MQ, RabbitMQ, ...)
    /// and transforms them into domain events that can be consumed by <see cref="IDomainEventHandler{TEvent}"/>s.
    /// </summary>
    /// <seealso cref="IMessageTransport"/>
    public interface IMessageReceiver
    {
        /// <summary>
        /// Handles an incoming message.
        /// </summary>
        /// <param name="eventType">The type of event that has arrived.</param>
        /// <param name="payload">The event in it's serialized form, should be accepted by <see cref="IDomainEventSerializer"/>.</param>
        /// <returns>A task that resolves once the <see cref="IDomainEventHandler{TEvent}"/>(s) are finished.</returns>
        /// <exception cref="PoisonedMessageException">Thrown when the received message is invalid and/or cannot be processed (eg. is poisoned).</exception>
        /// <exception cref="DomainEventHandlerException">Thrown when the <see cref="IDomainEventHandler{TEvent}"/> throws an exception processing the event.</exception>
        Task HandleIncoming(Type eventType, byte[] payload);
    }
}