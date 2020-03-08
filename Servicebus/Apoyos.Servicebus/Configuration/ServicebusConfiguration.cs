#pragma warning disable CA2227 // Configuration can contain mutable generics.
using System;
using System.Collections.Generic;

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

        /// <summary>
        /// The events that the servicebus can dispatch and/or respond to.
        /// </summary>
        /// <example>{"user.create", CreateUserEvent}</example>
        public Dictionary<string, Type> Events { get; set; } = new Dictionary<string, Type>();
    }
}