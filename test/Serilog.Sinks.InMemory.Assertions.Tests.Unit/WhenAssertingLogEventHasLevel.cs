using System;
using FluentAssertions;
using Serilog.Events;
using Xunit;
using Xunit.Sdk;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingLogEventHasLevel
    {
        private readonly ILogger _logger;

        public WhenAssertingLogEventHasLevel()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .CreateLogger();
        }

        [Fact]
        public void GivenInformationMessageIsLoggedAndAssertingWarning_WithLevelFails()
        {
            _logger.Information("Hello, world!");

            Action action = () => InMemorySink
                .Instance
                .Should()
                .HaveMessage("Hello, world!")
                .Appearing().Once()
                .WithLevel(LogEventLevel.Warning);

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected message \"Hello, world!\" to have level Warning, but it is Information");
        }

        [Fact]
        public void GivenInformationMessageIsLoggedAndAssertingInformation_WithLevelSucceeds()
        {
            _logger.Information("Hello, world!");

            InMemorySink
                .Instance
                .Should()
                .HaveMessage("Hello, world!")
                .Appearing().Once()
                .WithLevel(LogEventLevel.Information);
        }
    }
}