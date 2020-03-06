#pragma warning disable CA1063 // Dispose pattern implementation.
using System;
using System.Threading;
using System.Threading.Tasks;
using Apoyos.Servicebus.RabbitMQ.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Apoyos.Servicebus.RabbitMQ.Services
{
    /// <summary>
    /// Manages the physical (TCP) connection to RabbitMQ, as well as distributing channels (virtual connections).
    /// </summary>
    public class ConnectionService : IDisposable
    {
        private IConnection? _connection = null;
        private readonly RabbitMqConfiguration _configuration;

        /// <summary>
        /// Create a new instance of <see cref="ConnectionService"/>.
        /// </summary>
        public ConnectionService(IOptions<RabbitMqServicebusConfiguration> options)
        {
            _configuration = options.Value.RabbitMQ;
        }

        /// <summary>
        /// Attempt to open a connection to the RabbitMQ server.
        /// </summary>
        /// <exception cref="BrokerUnreachableException">When the configured hostname was not reachable.</exception>
        public Task ConnectAsync()
        {
            _connection = new ConnectionFactory
            {
                HostName = _configuration.Hostname,
                VirtualHost = _configuration.VirtualHost,
                UserName = _configuration.Username,
                Password = _configuration.Password,
                DispatchConsumersAsync = true
            }.CreateConnection();

            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Retrieve a channel (virtual connection) to RabbitMQ.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the connection was not opened (with <see cref="ConnectAsync"/>) or was already closed (with <see cref="Dispose"/>).</exception>
        public IModel GetChannel()
        {
            if (_connection?.IsOpen != true)
            {
                throw new InvalidOperationException("Cannot retrieve channel from closed connection.");
            }

            return _connection.CreateModel();
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            _connection?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}