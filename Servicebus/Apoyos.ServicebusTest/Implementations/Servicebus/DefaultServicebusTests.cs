#pragma warning disable CS1591 // Missing XML comment
#pragma warning disable CA2007 // ConfigureAwait
#pragma warning disable CS8618 // Nullable property not initialized
using Xunit;
using Apoyos.Servicebus.Implementations.Servicebus;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Apoyos.Servicebus.Abstractions.Models;
using Apoyos.Servicebus.Configuration;
using Apoyos.Servicebus.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Apoyos.Servicebus.Implementations.Servicebus.Tests
{
    public class DefaultServicebusTests
    {
        private readonly Mock<ILogger> _logger;
        private readonly Mock<IOptionsMonitor<ServicebusConfiguration>> _config;
        private readonly Mock<IDomainEventSerializer> _serializer;
        private readonly Mock<IMessageTransport> _transport;
        private readonly DefaultServicebus _bus;
        private const string serviceName = "XUNIT";
        private const string domainEventName = "xunit.dummyevent";

        public DefaultServicebusTests()
        {
            _logger = new Mock<ILogger>();
            _config = new Mock<IOptionsMonitor<ServicebusConfiguration>>();
            _serializer = new Mock<IDomainEventSerializer>();
            _transport = new Mock<IMessageTransport>();
            _bus = new DefaultServicebus(_logger.Object, _config.Object, _serializer.Object, _transport.Object);
            
            _config.Setup(o => o.CurrentValue).Returns(new ServicebusConfiguration
            {
                ServiceName = serviceName,
                Events = new Dictionary<string, string>
                {
                    {domainEventName, typeof(DummyEvent).Name}
                }
            });
        }
        
        [Fact]
        public void DefaultServicebusTest()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task PublishAsyncTest()
        {
            var domainEvent = new DummyEvent {Name = "John Doe"};
            var serializedEvent = new byte[] {1, 1, 1};

            // Setup mocks
            _serializer.Setup(s => s.SerializeAsync(It.IsAny<MessageMetadata<DummyEvent>>(), default))
                .Returns(Task.FromResult(serializedEvent));
            _transport.Setup(t => t.SendMessageAsync(domainEventName, serializedEvent)).Returns(Task.CompletedTask);

            // Execute tests
            await _bus.PublishAsync(domainEvent);
            _transport.Verify();
        }

        private class DummyEvent
        {
            public string Name { get; set; }
        }
    }
}