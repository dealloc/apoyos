using System;
using System.Threading;
using System.Threading.Tasks;
using Apoyos.Servicebus.Contracts;
using Apoyos.Servicebus.Exceptions;
using Apoyos.Servicebus.RabbitMQ.Configuration;
using Apoyos.Servicebus.RabbitMQ.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Apoyos.Servicebus.RabbitMQ.Hosted
{
    /// <summary>
    /// Handles the lifecycle of the ("physical") connection to the RabbitMQ server.
    /// </summary>
    public class HostedRabbitMqService : BackgroundService, IMessageTransport
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IConnectionService _connectionService;
        private readonly IMessageReceiver _messageReceiver;

        /// <summary>
        /// Create new instance of <see cref="HostedRabbitMqService"/>.
        /// </summary>
        public HostedRabbitMqService(IOptions<RabbitMqServicebusConfiguration> options, ILogger<HostedRabbitMqService> logger, IConnectionService connectionService, IMessageReceiver messageReceiver)
        {
            _configuration = options.Value.RabbitMQ;
            _logger = logger;
            _connectionService = connectionService;
            _messageReceiver = messageReceiver;
        }
        
        /// <inheritdoc cref="BackgroundService.ExecuteAsync" />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _connectionService.ConnectAsync().ConfigureAwait(false);

            // TODO: basic verification that every registered event has a queue, otherwise the event would never fire (or cannot be fired).
            foreach (var (eventType, queueName) in _configuration.Queues)
            {
                var channel = _connectionService.GetChannel(); // Note the absence of "using" statement here, since we want to keep the channels around. They are automatically cleaned up when the connection closes.
                var backoutQueue = $"{queueName}.BACKOUT";

                // Channels are bound to the last queue made on them, so we create the backout queue first.
                channel.QueueDeclare(queue: backoutQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                channel.QueueDeclare(queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                
                _logger.LogDebug("Bound event '{EventName}' to {QueueName} (backout -> {BackoutQueue}", eventType.FullName, queueName, backoutQueue);
                
                // TODO: if we're only going to be publishing an event, we don't need to register a listener for it.
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (sender, message) =>
                {
                    try
                    {
                        _logger.LogDebug("Incoming message on {QueueName}", queueName);
                        await _messageReceiver.HandleIncoming(eventType, message.Body).ConfigureAwait(false);
                    }
                    catch (Exception  exception) when (exception is PoisonedMessageException || exception is DomainEventHandlerException)
                    {
                        _logger.LogWarning(exception, "Failed to process {ConsumerTag}+{DeliveryTag} due to exception.",
                            message.ConsumerTag, message.DeliveryTag);

                        channel.BasicPublish(
                            exchange: string.Empty, // Default exchange.
                            routingKey: backoutQueue,
                            basicProperties: channel.CreateBasicProperties(),
                            body: message.Body);
                    }
                    finally
                    {
                        channel.BasicAck(message.DeliveryTag, multiple: false);
                    }
                };

                channel.BasicQos(0, prefetchCount: 1, global: false); // Only preload one message at a time.
                channel.BasicConsume(queueName, autoAck: false, consumer);
            }

            // Wait until stoppingToken indicates the service should stop.
            await Task.Delay(-1, stoppingToken).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IMessageTransport.SendMessageAsync" />
        public Task SendMessageAsync<TEvent>(byte[] message) where TEvent : class, new()
        {
            // TODO: creating a new channel for each message is expensive.
            using var channel = _connectionService.GetChannel();
            var queueName = _configuration.Queues[typeof(TEvent)];

            channel.BasicPublish(
                exchange: string.Empty, // Default exchange.
                routingKey: queueName,
                basicProperties: channel.CreateBasicProperties(),
                body: message);
            
            return Task.CompletedTask;
        }
    }
}