using System.Threading.Tasks;

namespace Apoyos.Servicebus.Contracts
{
    /// <summary>
    /// Allows publishing domain events that can be consumed by the registered consumers.
    /// </summary>
    /// <seealso cref="IDomainEventHandler{TEvent}"/>
    public interface IServicebus
    {
        /// <summary>
        /// Publish an event on the bus asynchronously.
        /// </summary>
        /// <param name="domainEvent">The event to publish on the bus.</param>
        /// <typeparam name="TEvent">The type of event to publish.</typeparam>
        /// <returns>A <see cref="Task"/> that resolves once the message has been posted on the bus.</returns>
        /// <remarks>Note that the returned task resolving does not mean it has been consumed, only that it has been sent.</remarks>
        Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : class, new();
    }
}
