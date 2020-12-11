#pragma warning disable CA1063 // Dispose pattern implementation.
using System;
using System.Threading.Tasks;
using Apoyos.Servicebus.Configuration;
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
        private IConnection? _connection;
        private readonly ILogger<ConnectionService> _logger;
        private readonly RabbitMqServicebusConfiguration _configuration;
        private readonly ServicebusConfiguration _servicebusOptions;

        /// <summary>
        /// Create a new instance of <see cref="ConnectionService"/>.
        /// </summary>
        public ConnectionService(IOptions<RabbitMqServicebusConfiguration> options, ILogger<ConnectionService> logger)
        {
            _logger = logger;
            _configuration = options.Value;
        }

        /// <inheritdoc cref="IConnectionService.ConnectAsync" />
        public Task ConnectAsync()
        {
            _logger.LogDebug("Opening new connection to {Hostname}{VirtualHost}", _configuration.RabbitMQ.Hostname, _configuration.RabbitMQ.VirtualHost);
            _connection = new ConnectionFactory
            {
                HostName = _configuration.RabbitMQ.Hostname,
                VirtualHost = _configuration.RabbitMQ.VirtualHost,
                UserName = _configuration.RabbitMQ.Username,
                Password = _configuration.RabbitMQ.Password,
                ClientProvidedName = _servicebusOptions.ServiceName,
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