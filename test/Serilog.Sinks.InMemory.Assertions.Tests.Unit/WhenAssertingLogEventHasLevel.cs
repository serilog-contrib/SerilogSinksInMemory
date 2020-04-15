using System;
using FluentAssertions;
using NUnit.Framework;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingLogEventHasLevel
    {
        private readonly Logger _logger;

        public WhenAssertingLogEventHasLevel()
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
        public void GivenInformationMessageIsLoggedAndAssertingWarning_WithLevelFails()
        {
            _logger.Information("Hello, world!");

            Action action = () => InMemorySink.Instance
                .Should()
                .HaveMessage("Hello, world!")
                .Appearing().Once()
                .WithLevel(LogEventLevel.Warning);

            action
                .Should()
                .Throw<Exception>()
                .WithMessage("Expected message \"Hello, world!\" to have level Warning, but it is Information");
        }

        [Test]
        public void GivenInformationMessageIsLoggedAndAssertingInformation_WithLevelSucceeds()
        {
            _logger.Information("Hello, world!");
            
            InMemorySink.Instance
                .Should()
                .HaveMessage("Hello, world!")
                .Appearing().Once()
                .WithLevel(LogEventLevel.Information);
        }

        [Test]
        public void GivenMultipleInformationMessagesAndAssertingInformation_WithLevelSucceeds()
        {
            _logger.Information("Hello, world!");
            _logger.Information("Hello, world!");
            _logger.Information("Hello, world!");

            InMemorySink.Instance
                .Should()
                .HaveMessage("Hello, world!")
                .Appearing().Times(3)
                .WithLevel(LogEventLevel.Information);
        }

        [Test]
        public void GivenMultipleWarningMessagesAndAssertingInformation_WithLevelFails()
        {
            _logger.Warning("Hello, world!");
            _logger.Warning("Hello, world!");
            _logger.Warning("Hello, world!");

            Action action = () => InMemorySink.Instance
                .Should()
                .HaveMessage("Hello, world!")
                .Appearing().Times(3)
                .WithLevel(LogEventLevel.Information);

            action
                .Should()
                .Throw<Exception>()
                .WithMessage("Expected instances of log message \"Hello, world!\" to have level Information, but found 3 with level Warning");
        }
    }
}