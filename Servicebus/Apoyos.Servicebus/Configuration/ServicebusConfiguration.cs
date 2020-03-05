#pragma warning disable CA2227 // Configuration can contain mutable generics.
using System;
using System.Collections.Generic;
using Apoyos.Servicebus.Extensions;

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
        /// To add events, see <see cref="ServiceCollectionExtensions.AddDomainEvent{TEvent}"/> and <see cref="ServiceCollectionExtensions.AddDomainEventListener{TEvent,THandler}"/>
        /// </summary>
        /// <example>{"user.create", CreateUserEvent}</example>
        public IReadOnlyDictionary<string, Type> Events => _events;
        
        /// <summary>
        /// The events that the servicebus can dispatch and/or respond to.
        /// </summary>
        /// <seealso cref="Events"/>
        internal readonly Dictionary<string, Type> _events = new Dictionary<string, Type>();
    }
}