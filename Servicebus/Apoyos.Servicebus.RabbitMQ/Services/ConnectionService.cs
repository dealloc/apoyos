#pragma warning disable CA1063 // Dispose pattern implementation.
using System;
using System.Threading.Tasks;
using Apoyos.Servicebus.RabbitMQ.Configuration;
using Apoyos.Servicebus.RabbitMQ.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Apoyos.Servicebus.RabbitMQ.Services
{
    /// <summary>
    /// Manages the physical (TCP) connection to RabbitMQ, as well as distributing channels (virtual connections).
    /// </summary>
    public class ConnectionService : IConnectionService
    {
        private readonly ILogger<ConnectionService> _logger;
        private IConnection? _connection = null;
        private readonly RabbitMqConfiguration _configuration;

        /// <summary>
        /// Create a new instance of <see cref="ConnectionService"/>.
        /// </summary>
        public ConnectionService(IOptions<RabbitMqServicebusConfiguration> options, ILogger<ConnectionService> logger)
        {
            _logger = logger;
            _configuration = options.Value.RabbitMQ;
        }

        /// <inheritdoc cref="IConnectionService.ConnectAsync" />
        public Task ConnectAsync()
        {
            _logger.LogDebug("Opening new connection to {Hostname}{VirtualHost}", _configuration.Hostname, _configuration.VirtualHost);
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
        
        /// <inheritdoc cref="IConnectionService.GetChannel" />
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