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
                .WithMessage("Expected message \"Hello, World\" to be logged");
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
        public void GivenTheSameLogMessageFourTimesAndAssertingOnlyOnce_HaveMessageFails()
        {
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");

            Action action = () => _sink
                .Should()
                .HaveMessage("Hello, World")
                .Appearing().Once();

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected message \"Hello, World\" to appear exactly once, but it was found 4 times");
        }

        [Fact]
        public void GivenTheSameLogMessageFourTimesAndAssertingFourTimes_HaveMessageSucceeds()
        {
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");

            _sink
                .Should()
                .HaveMessage("Hello, World")
                .Appearing().Times(4);
        }

        [Fact]
        public void GivenTheSameLogMessageFourTimesAndAssertingFiveTimes_HaveMessageFails()
        {
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");

            Action action = () => _sink
                .Should()
                .HaveMessage("Hello, World")
                .Appearing().Times(5);

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected message \"Hello, World\" to appear 5 times, but it was found 4 times");
        }
    }
}