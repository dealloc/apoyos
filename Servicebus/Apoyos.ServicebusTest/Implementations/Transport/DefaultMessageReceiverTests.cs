﻿#pragma warning disable CS1591 // Missing XML comment
#pragma warning disable CA2007 // ConfigureAwait
#pragma warning disable CS8618 // Nullable property not initialized
#pragma warning disable CA1034 // Do not nest DummyEvent
using System;
using System.Threading.Tasks;
using Apoyos.Servicebus.Abstractions.Models;
using Apoyos.Servicebus.Configuration;
using Apoyos.Servicebus.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Apoyos.Servicebus.Implementations.Transport.Tests
{
    public class DefaultMessageReceiverTests
    {
        private readonly Mock<IOptions<ServicebusConfiguration>> _options;
        private readonly Mock<ILogger<DefaultMessageReceiver>> _logger;
        private readonly ServiceCollection _services;
        private readonly Mock<IDomainEventSerializer> _serializer;
        private readonly Mock<IDomainEventHandler<DummyEvent>> _handler;
        private ReadOnlyMemory<byte> _serializedDomainEvent;
        private DummyEvent _domainEvent;
        private const string ServiceName = "XUNIT";

        public DefaultMessageReceiverTests()
        {
            _options = new Mock<IOptions<ServicebusConfiguration>>();
            _logger = new Mock<ILogger<DefaultMessageReceiver>>();
            _services = new ServiceCollection();
            _serializer = new Mock<IDomainEventSerializer>();
            _handler = new Mock<IDomainEventHandler<DummyEvent>>();
            _serializedDomainEvent = new ReadOnlyMemory<byte>(new byte[] { 1, 1, 1 });
            _domainEvent = new DummyEvent {Name = "Hello world"};

            _services.AddTransient(p => _handler.Object);
            _options.Setup(o => o.Value).Returns(new ServicebusConfiguration
            {
                ServiceName = ServiceName
            });
        }
        
        [Fact]
        public async Task HandleIncomingTest()
        {
            var metadata = new MessageMetadata<DummyEvent>(ServiceName, _domainEvent);
            var receiver = new DefaultMessageReceiver(_options.Object, _logger.Object, _services.BuildServiceProvider(), _serializer.Object);

            _serializer.Setup(s => s.DeserializeAsync<MessageMetadata<DummyEvent>>(_serializedDomainEvent, default))
                .Returns(Task.FromResult(metadata));
            _handler.Setup(h => h.HandleAsync(_domainEvent)).Verifiable();

            await receiver.HandleIncoming(typeof(DummyEvent), _serializedDomainEvent);
            _handler.Verify();
        }

        [Fact]
        public async Task handleIncomingWaitsForHandlers()
        {
            var receiver = new DefaultMessageReceiver(_options.Object, _logger.Object, _services.BuildServiceProvider(), _serializer.Object);
            var handlerTask = new TaskCompletionSource<int>();

            _serializer.Setup(s => s.DeserializeAsync<MessageMetadata<DummyEvent>>(_serializedDomainEvent, default))
                .Returns(Task.FromResult(new MessageMetadata<DummyEvent>(ServiceName, _domainEvent)));
            _handler.Setup(h => h.HandleAsync(It.IsAny<DummyEvent>())).Returns(handlerTask.Task);
            var handleIncomingTask = receiver.HandleIncoming(typeof(DummyEvent), _serializedDomainEvent);

            Assert.False(handleIncomingTask.IsCompleted);
            handlerTask.SetResult(1); // Simulate completion of the handler.
            await Task.Delay(TimeSpan.FromMilliseconds(100)); // Give HandleIncoming a chance to complete.
            Assert.True(handleIncomingTask.IsCompleted);
        }

        public class DummyEvent
        {
            public string Name { get; set; }
        }
    }
}