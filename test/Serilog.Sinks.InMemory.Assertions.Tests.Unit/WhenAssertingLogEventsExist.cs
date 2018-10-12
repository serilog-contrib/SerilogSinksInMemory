using System;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingLogEventsExist
    {
        private readonly InMemorySink _sink;
        private readonly ILogger _logger;

        public WhenAssertingLogEventsExist()
        {
            _sink = new InMemorySink();
            _logger = new LoggerConfiguration()
                .WriteTo.Sink(_sink)
                .CreateLogger();
        }

        [Fact]
        public void GivenNoLogMessage_HaveMessageFails()
        {
            Action action = () => _sink
                .Should()
                .HaveMessage("Hello, World");

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected a message to be logged with template \"Hello, World\" but didn't find any");
        }

        [Fact]
        public void GivenSingleLogMessage_HaveMessagePasses()
        {
            _logger.Information("Hello, World");

            _sink
                .Should()
                .HaveMessage("Hello, World");
        }

        [Fact]
        public void GivenTheSameLogMessageMultipleTimes_HaveMessageFails()
        {
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");

            Action action = () => _sink
                .Should()
                .HaveMessage("Hello, World");

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected a message to be logged with template \"Hello, World\" exactly once, but it was found 4 times");
        }
    }
}