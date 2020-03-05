using System;
using Apoyos.Servicebus.Contracts;

namespace Apoyos.Servicebus.Exceptions
{
    /// <summary>
    /// Thrown when an <see cref="IDomainEventHandler{TEvent}"/> throws an exception while handling an incoming event.
    /// </summary>
    public class DomainEventHandlerException : Exception
    {
        /// <summary>
        /// Create a new instance of <see cref="DomainEventHandlerException"/>.
        /// </summary>
        public DomainEventHandlerException()
        {
            //
        }

        /// <summary>
        /// Create a new instance of <see cref="DomainEventHandlerException"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        public DomainEventHandlerException(string message) : base(message)
        {
            //
        }

        /// <summary>
        /// Create a new instance of <see cref="DomainEventHandlerException"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        public DomainEventHandlerException(string message, Exception innerException) : base(message, innerException)
        {
            //
        }
    }
}