using System;

namespace Apoyos.Servicebus.Abstractions.Models
{
    /// <summary>
    /// Contains the metadata of <typeparamref name="TEvent"/> while it's in transit.
    /// </summary>
    /// <typeparam name="TEvent">The event for which metadata is being provided.</typeparam>
    public class MessageMetadata<TEvent> where TEvent : class, new()
    {
        /// <summary>
        /// The identifier of this specific event, allows tracing it across multiple services.
        /// </summary>
        /// <remarks>While this will usually be a GUID, there should be no assumptions it is.</remarks>
        public string Identifier { get; set; } = string.Empty;

        /// <summary>
        /// The human readable name of the service from which the event originated.
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the event was originally created.
        /// </summary>
        public DateTime CreatedOn { get; set; } = DateTime.UnixEpoch;
        
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
        public MessageMetadata(string createdBy, TEvent domainEvent)
        {
            Identifier = Guid.NewGuid().ToString();
            CreatedBy = createdBy;
            CreatedOn = DateTime.UtcNow;
            Event = domainEvent;
        }
    }
}