using System;

namespace Apoyos.Servicebus.Abstractions.Models
{
    /// <summary>
    /// Contains the metadata of <typeparamref name="TEvent"/> while it's in transit.
    /// </summary>
    /// <typeparam name="TEvent">The event for which metadata is being provided.</typeparam>
    public class MessageMetadata<TEvent> : BaseMessageMetadata where TEvent : class, new()
    {
        /// <summary>
        /// The event for which metadata is being provided.
        /// </summary>
        public TEvent? Event { get; set; }

        /// <summary>
        /// The default constructor for <see cref="MessageMetadata{TEvent}"/>.
        /// </summary>
        [Obsolete("This method should only be used by serialization logic and not directly.")]
        public MessageMetadata()
        {
            //
        }

        /// <summary>
        /// Create metadata for a given <typeparamref name="TEvent"/>.
        /// </summary>
        /// <param name="createdBy">The human readable name of the service that created <paramref name="domainEvent"/>.</param>
        /// <param name="domainEvent">The domain event for which metadata is being provided.</param>
        public MessageMetadata(string createdBy, TEvent domainEvent) : base(createdBy)
        {
            Event = domainEvent;
        }
    }
}