#pragma warning disable CA2227
using System.Collections.Generic;

namespace Apoyos.Servicebus.RabbitMQ.Configuration
{
    /// <summary>
    /// Contains the configuration of RabbitMQ.
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
        /// The username to authenticate to RabbitMQ.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The password to authenticate to RabbitMQ.
        /// </summary>
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// Configures which queues match to which domain event.
        /// </summary>
        /// <example>{"EVENTNAME", "QUEUENAME"}</example>
        public Dictionary<string, string> Queues { get; set; } = new Dictionary<string, string>();
    }
}