using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Apoyos.Servicebus.RabbitMQ.Contracts
{
    /// <summary>
    /// Manages the physical (TCP) connection to RabbitMQ, as well as distributing channels (virtual connections).
    /// </summary>
    public interface IConnectionService : IDisposable
    {
        /// <summary>
        /// Attempt to open a connection to the RabbitMQ server.
        /// </summary>
        /// <exception cref="BrokerUnreachableException">When the configured hostname was not reachable.</exception>
        Task ConnectAsync();

        /// <summary>
        /// Retrieve a channel (virtual connection) to RabbitMQ.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the connection was not opened (with <see cref="ConnectAsync"/>) or was already closed (with <see cref="IDisposable.Dispose"/>).</exception>
        IModel GetChannel();
    }
}