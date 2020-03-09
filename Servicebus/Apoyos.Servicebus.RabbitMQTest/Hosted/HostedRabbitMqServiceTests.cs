#pragma warning disable CS1591 // Missing XML comment
#pragma warning disable CA2007 // ConfigureAwait
#pragma warning disable CS8618 // Nullable property not initialized
#pragma warning disable CA1034 // Do not nest DummyEvent
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Apoyos.Servicebus.Contracts;
using Apoyos.Servicebus.Exceptions;
using Apoyos.Servicebus.RabbitMQ.Configuration;
using Apoyos.Servicebus.RabbitMQ.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using Xunit;

namespace Apoyos.Servicebus.RabbitMQ.Hosted.Tests
{
    public class HostedRabbitMqServiceTests
    {
        private readonly Mock<IOptions<RabbitMqServicebusConfiguration>> _options;
        private readonly Mock<ILogger<HostedRabbitMqService>> _logger;
        private readonly Mock<IConnectionService> _connectionService;
        private readonly Mock<IMessageReceiver> _messageReceiver;
        private const string ServiceName = "XUNIT";
        private const string RabbitMQVirtualHost = "/xunit";
        private const string RabbitMQHostname = "xunit";
        private const string RabbitMQUsername = "username";
        private const string RabbitMQPassword = "emanresu";
        private const string QueueName = "XUNIT.DUMMY";
        
        public HostedRabbitMqServiceTests()
        {
            _options = new Mock<IOptions<RabbitMqServicebusConfiguration>>();
            _logger = new Mock<ILogger<HostedRabbitMqService>>();
            _connectionService = new Mock<IConnectionService>();
            _messageReceiver = new Mock<IMessageReceiver>();

            _options.Setup(o => o.Value).Returns(new RabbitMqServicebusConfiguration
            {
                ServiceName = ServiceName,
                RabbitMQ = new RabbitMqConfiguration
                {
                    Hostname = RabbitMQHostname,
                    Username = RabbitMQUsername,
                    Password = RabbitMQPassword,
                    VirtualHost = RabbitMQVirtualHost,
                    Queues =
                    {
                        {
                            typeof(DummyEvent), new QueueConfiguration
                            {
                                QueueName = QueueName,
                                Listen = true
                            }
                        }
                    }
                }
            });
        }
        
        [Fact]
        public async Task DeclaresQueuesOnStartup()
        {
            var channelMock = new Mock<IModel>();
            using var hosted = new HostedRabbitMqService(_options.Object, _logger.Object, _connectionService.Object, _messageReceiver.Object);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            _connectionService.Setup(c => c.GetChannel()).Returns(channelMock.Object);
            channelMock.Setup(channel => channel.QueueDeclare(QueueName, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<Dictionary<string, object>>()))
                .Verifiable($"Queue {QueueName} did not get declared!");
            
            await hosted.StartAsync(cancellation.Token);
            channelMock.Verify();
        }
        
        [Fact]
        public async Task ConsumerTriggersMessageReceiver()
        {
            var channelMock = new Mock<IModel>();
            var simulatedPayload = new byte[] {1, 1, 1};
            AsyncEventingBasicConsumer? basicConsumer = null; // Once the channelMock registers a consumer, we'll fill it in here.
            using var hosted = new HostedRabbitMqService(_options.Object, _logger.Object, _connectionService.Object, _messageReceiver.Object);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            _connectionService.Setup(c => c.GetChannel()).Returns(channelMock.Object);
            
            // in reality an extension method with a simpler signature is called, but Moq won't allow us to simulate that directly. So we expand the called extension method here.
            channelMock.Setup(c => c.BasicConsume(QueueName, It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<AsyncEventingBasicConsumer>()))
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>((name, autoAck, consumerTag, noLocal, exclusive, arguments, consumer) =>
                {
                    // Consumer here is an actual RabbitMQ consumer waiting for our model to fire stuff.
                    basicConsumer = consumer as AsyncEventingBasicConsumer;
                });
            _messageReceiver.Setup(m => m.HandleIncoming(typeof(DummyEvent), simulatedPayload)).Returns(Task.CompletedTask).Verifiable("Message receiver was not called!");
            
            await hosted.StartAsync(cancellation.Token);
            basicConsumer?.HandleBasicDeliver(string.Empty, 420, false, string.Empty, QueueName, new BasicProperties(), simulatedPayload);

            channelMock.Verify();
            _messageReceiver.Verify();
        }

        [Fact]
        public async Task DomainEventHandlerErrorMovesMessageToBackout()
        {
            var channelMock = new Mock<IModel>();
            var simulatedPayload = new byte[] {1, 1, 1};
            AsyncEventingBasicConsumer? basicConsumer = null; // Once the channelMock registers a consumer, we'll fill it in here.
            using var hosted = new HostedRabbitMqService(_options.Object, _logger.Object, _connectionService.Object, _messageReceiver.Object);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            _connectionService.Setup(c => c.GetChannel()).Returns(channelMock.Object);
            
            // in reality an extension method with a simpler signature is called, but Moq won't allow us to simulate that directly. So we expand the called extension method here.
            channelMock.Setup(c => c.BasicConsume(QueueName, It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<AsyncEventingBasicConsumer>()))
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>((name, autoAck, consumerTag, noLocal, exclusive, arguments, consumer) =>
                {
                    // Consumer here is an actual RabbitMQ consumer waiting for our model to fire stuff.
                    basicConsumer = consumer as AsyncEventingBasicConsumer;
                });

            _messageReceiver.Setup(m => m.HandleIncoming(typeof(DummyEvent), simulatedPayload))
                .Throws<DomainEventHandlerException>();
            
            channelMock.Setup(c => c.BasicPublish(string.Empty, $"{QueueName}.BACKOUT", false, null, simulatedPayload)).Verifiable("Backout message was not posted!");
            
            await hosted.StartAsync(cancellation.Token);
            basicConsumer?.HandleBasicDeliver(string.Empty, 420, false, string.Empty, QueueName, new BasicProperties(), simulatedPayload);

            channelMock.Verify();
            _messageReceiver.Verify();
        }

        [Fact]
        public async Task PoisonedMessageErrorMovesMessageToBackout()
        {
            var channelMock = new Mock<IModel>();
            var simulatedPayload = new byte[] {1, 1, 1};
            AsyncEventingBasicConsumer? basicConsumer = null; // Once the channelMock registers a consumer, we'll fill it in here.
            using var hosted = new HostedRabbitMqService(_options.Object, _logger.Object, _connectionService.Object, _messageReceiver.Object);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            _connectionService.Setup(c => c.GetChannel()).Returns(channelMock.Object);
            
            // in reality an extension method with a simpler signature is called, but Moq won't allow us to simulate that directly. So we expand the called extension method here.
            channelMock.Setup(c => c.BasicConsume(QueueName, It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<AsyncEventingBasicConsumer>()))
                .Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>((name, autoAck, consumerTag, noLocal, exclusive, arguments, consumer) =>
                {
                    // Consumer here is an actual RabbitMQ consumer waiting for our model to fire stuff.
                    basicConsumer = consumer as AsyncEventingBasicConsumer;
                });

            _messageReceiver.Setup(m => m.HandleIncoming(typeof(DummyEvent), simulatedPayload))
                .Throws<PoisonedMessageException>();
            
            channelMock.Setup(c => c.BasicPublish(string.Empty, $"{QueueName}.BACKOUT", false, null, simulatedPayload)).Verifiable("Backout message was not posted!");
            
            await hosted.StartAsync(cancellation.Token);
            basicConsumer?.HandleBasicDeliver(string.Empty, 420, false, string.Empty, QueueName, new BasicProperties(), simulatedPayload);

            channelMock.Verify();
            _messageReceiver.Verify();
        }

        public class DummyEvent
        {
            public string Name { get; set; }
        }
    }
}