using System;

namespace Apoyos.Servicebus.Exceptions
{
    /// <summary>
    /// Thrown when the received message is invalid and/or cannot be processed (eg. is poisoned).
    /// </summary>
    public class PoisonedMessageException : Exception
    {
        /// <summary>
        /// Create a new instance of <see cref="PoisonedMessageException"/>.
        /// </summary>
        public PoisonedMessageException()
        {
            //
        }

        /// <summary>
        /// Create a new instance of <see cref="PoisonedMessageException"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        public PoisonedMessageException(string message) : base(message)
        {
            //
        }

        /// <summary>
        /// Create a new instance of <see cref="PoisonedMessageException"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        public PoisonedMessageException(string message, Exception innerException) : base(message, innerException)
        {
            //
        }
    }
}