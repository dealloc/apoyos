using Apoyos.Servicebus.Configuration;

namespace Apoyos.Servicebus.RabbitMQ.Configuration
{
    /// <summary>
    /// Contains the configuration of the servicebus for RabbitMQ.
    /// </summary>
    public class RabbitMqServicebusConfiguration : ServicebusConfiguration
    {
        /// <summary>
        /// The configuration for RabbitMQ.
        /// </summary>
        public RabbitMqConfiguration RabbitMQ { get; set; } = new RabbitMqConfiguration();
    }
}