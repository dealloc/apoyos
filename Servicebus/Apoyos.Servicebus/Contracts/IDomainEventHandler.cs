using System.Threading.Tasks;

namespace Apoyos.Servicebus.Contracts
{
    /// <summary>
    /// Handles incoming <typeparamref name="TEvent"/> domain events.
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event this handler processes.</typeparam>
    public interface IDomainEventHandler<TEvent> where TEvent : class, new()
    {
        /// <summary>
        /// Handles the incoming <typeparamref name="TEvent"/> event.
        /// </summary>
        /// <param name="domainEvent">The domain event to handle.</param>
        /// <returns>A task that resolves once the event is considered "handled".</returns>
        Task HandleAsync(TEvent domainEvent);
    }
}