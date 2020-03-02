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
        /// <param name="eventName">The name of the event being sent.</param>
        /// <param name="message">The message being sent to other services.</param>
        /// <returns>A <see cref="Task"/> that resolves once the message has been posted.</returns>
        /// <remarks>The <paramref name="eventName"/> is used to determine where the event should be posted to.</remarks>
        Task SendMessageAsync(string eventName, byte[] message);
    }
}