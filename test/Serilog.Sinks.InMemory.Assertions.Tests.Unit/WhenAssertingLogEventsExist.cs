using System;
using FluentAssertions;
using NUnit.Framework;
using Serilog.Core;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingLogEventsExist
    {
        private readonly Logger _logger;

        public WhenAssertingLogEventsExist()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .CreateLogger();
        }

        [TearDown]
        public void TearDown()
        {
            // Instance isolation gets tricky with tests so we need to 
            // ensure the sink is reset after each test.
            _logger.Dispose();
        }

        [Test]
        public void GivenNoLogMessage_HaveMessageFails()
        {
            Action action = () => InMemorySink.Instance
                .Should()
                .HaveMessage("Hello, World");

            action
                .Should()
                .Throw<Exception>()
                .WithMessage("Expected message \"Hello, World\" to be logged");
        }

        [Test]
        public void GivenSingleLogMessage_HaveMessagePasses()
        {
            _logger.Information("Hello, World");

            InMemorySink.Instance
                .Should()
                .HaveMessage("Hello, World");
        }

        [Test]
        public void GivenTheSameLogMessageFourTimesAndAssertingOnlyOnce_HaveMessageFails()
        {
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");

            Action action = () => InMemorySink.Instance
                .Should()
                .HaveMessage("Hello, World")
                .Appearing().Once();

            action
                .Should()
                .Throw<Exception>()
                .WithMessage("Expected message \"Hello, World\" to appear exactly once, but it was found 4 times");
        }

        [Test]
        public void GivenTheSameLogMessageFourTimesAndAssertingFourTimes_HaveMessageSucceeds()
        {
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");

            InMemorySink.Instance
                .Should()
                .HaveMessage("Hello, World")
                .Appearing().Times(4);
        }

        [Test]
        public void GivenTheSameLogMessageFourTimesAndAssertingFiveTimes_HaveMessageFails()
        {
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");
            _logger.Information("Hello, World");

            Action action = () => InMemorySink.Instance
                .Should()
                .HaveMessage("Hello, World")
                .Appearing().Times(5);

            action
                .Should()
                .Throw<Exception>()
                .WithMessage("Expected message \"Hello, World\" to appear 5 times, but it was found 4 times");
        }
    }
}