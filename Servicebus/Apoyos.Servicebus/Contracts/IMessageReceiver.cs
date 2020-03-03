using System.Threading.Tasks;

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
        /// <param name="eventName">The name of the event that has arrived.</param>
        /// <param name="payload">The event in it's serialized form, should be accepted by <see cref="IDomainEventSerializer"/>.</param>
        /// <returns>A task that resolves once the <see cref="IDomainEventHandler{TEvent}"/>(s) are finished.</returns>
        Task HandleIncoming(string eventName, byte[] payload);
    }
}