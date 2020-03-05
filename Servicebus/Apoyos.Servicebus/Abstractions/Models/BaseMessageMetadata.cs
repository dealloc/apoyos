using System;

namespace Apoyos.Servicebus.Abstractions.Models
{
    /// <summary>
    /// Contains the metadata of a message while it's in transit.
    /// </summary>
    public class BaseMessageMetadata
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
        /// The default constructor for <see cref="BaseMessageMetadata"/>.
        /// </summary>
        [Obsolete("This method should only be used by serialization logic and not directly.")]
        public BaseMessageMetadata()
        {
            //
        }

        /// <summary>
        /// Create metadata for a given message.
        /// </summary>
        /// <param name="createdBy">The human readable name of the service that created the message.</param>
        public BaseMessageMetadata(string createdBy)
        {
            Identifier = Guid.NewGuid().ToString();
            CreatedBy = createdBy;
            CreatedOn = DateTime.UtcNow;
        }
    }
}