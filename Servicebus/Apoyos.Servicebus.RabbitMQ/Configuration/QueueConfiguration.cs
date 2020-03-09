#pragma warning disable CA2225 // No, I will not provide an alternative operator.
#pragma warning disable CS8625
#pragma warning disable CA2227
using System.Collections.Generic;
using RabbitMQ.Client;

namespace Apoyos.Servicebus.RabbitMQ.Configuration
{
    /// <summary>
    /// Configuration of a queue.
    /// </summary>
    public class QueueConfiguration
    {
        /// <summary>
        /// The name of the queue.
        /// </summary>
        public string QueueName { get; set; } = string.Empty;

        /// <summary>
        /// Whether to listen to incoming messages on the queue.
        /// </summary>
        public bool Listen { get; set; } = false;

        /// <summary>
        /// Passed directly into <see cref="IModel.QueueDeclare"/>.
        /// </summary>
        public IDictionary<string, object> Properties { get; set; } = default;

        /// <summary>
        /// Create a new <see cref="QueueConfiguration"/> from a <see cref="string"/> representing it's name.
        /// </summary>
        /// <param name="value">The <see cref="QueueName"/>.</param>
        public static implicit operator QueueConfiguration(string value) => new QueueConfiguration
        {
            QueueName = value
        };
    }
}