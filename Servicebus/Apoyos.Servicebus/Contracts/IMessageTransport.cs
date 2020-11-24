using System;
using System.Threading.Tasks;

namespace Apoyos.Servicebus.Contracts
{
    /// <summary>
    /// Handles the effective transport of messages from one service to services that can handle such event.
    /// </summary>
    public interface IMessageTransport
    {
        /// <summary>
        /// Send a message asynchronously.
        /// </summary>
        /// <param name="message">The message being sent to other services.</param>
        /// <typeparam name="TEvent">The type of message we're sending.</typeparam>
        /// <returns>A <see cref="Task"/> that resolves once the message has been posted.</returns>
        Task SendMessageAsync<TEvent>(ReadOnlyMemory<byte> message) where TEvent : class, new();
    }
}