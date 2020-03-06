using Apoyos.Servicebus.Configuration;

namespace Apoyos.Servicebus.RabbitMQ.Configuration
{
    /// <summary>
    /// Contains the configuration of the servicebus for RabbitMQ.
    /// </summary>
    public class RabbitMqServicebusConfiguration : ServicebusConfiguration
    {
        public RabbitMqConfiguration RabbitMQ { get; set; } = new RabbitMqConfiguration();
    }
}