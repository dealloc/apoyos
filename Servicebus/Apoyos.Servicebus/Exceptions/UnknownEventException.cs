using System;

namespace Apoyos.Servicebus.Exceptions
{
    /// <summary>
    /// Thrown when attempting to publish an event that wasn't configured.
    /// </summary>
    public class UnknownEventException : Exception
    {
        /// <summary>
        /// Create a new instance of <see cref="UnknownEventException"/>.
        /// </summary>
        public UnknownEventException()
        {
            //
        }

        /// <summary>
        /// Create a new instance of <see cref="UnknownEventException"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        public UnknownEventException(string message) : base(message)
        {
            //
        }

        /// <summary>
        /// Create a new instance of <see cref="UnknownEventException"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        public UnknownEventException(string message, Exception innerException) : base(message, innerException)
        {
            //
        }
    }
}