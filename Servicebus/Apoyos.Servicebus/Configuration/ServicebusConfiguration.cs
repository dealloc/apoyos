namespace Apoyos.Servicebus.Configuration
{
    /// <summary>
    /// Configures how the servicebus should behave.
    /// </summary>
    /// <remarks>This does not include configurations of transport etc.</remarks>
    public class ServicebusConfiguration
    {
        /// <summary>
        /// The name of the service consuming and dispatching events.
        /// </summary>
        public string ServiceName { get; set; } = string.Empty;
    }
}