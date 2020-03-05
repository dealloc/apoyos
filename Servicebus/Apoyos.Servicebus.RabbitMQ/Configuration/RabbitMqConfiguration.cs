#pragma warning disable CA2227
using System.Collections.Generic;

namespace Apoyos.Servicebus.RabbitMQ.Configuration
{
    /// <summary>
    /// Contains the configuration of the servicebus for RabbitMQ.
    /// </summary>
    public class RabbitMqConfiguration
    {
        /// <summary>
        /// The hostname of the RabbitMQ server.
        /// </summary>
        public string Hostname { get; set; } = string.Empty;

        /// <summary>
        /// The VirtualHost RabbitMQ to connect to.
        /// </summary>
        public string VirtualHost { get; set; } = "/";
        
        /// <summary>
        /// Configures which queues match to which domain event.
        /// </summary>
        /// <example>{"EVENTNAME", "QUEUENAME"}</example>
        public Dictionary<string, string> Queues { get; set; } = new Dictionary<string, string>();
    }
}