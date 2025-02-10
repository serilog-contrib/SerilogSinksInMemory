using System;
using Shouldly;
using Serilog.Events;
using Xunit;
using Xunit.Sdk;
using Serilog.Sinks.InMemory.Assertions;

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

            Should.Throw<XunitException>(() => action())
                .Message
                .ShouldBe("Expected message \"Hello, world!\" to have level \"Warning\", but it is \"Information\"");
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

        [Fact]
        public void GivenMultipleInformationMessagesAndAssertingInformation_WithLevelSucceeds()
        {
            _logger.Information("Hello, world!");
            _logger.Information("Hello, world!");
            _logger.Information("Hello, world!");

            InMemorySink
                .Instance
                .Should()
                .HaveMessage("Hello, world!")
                .Appearing().Times(3)
                .WithLevel(LogEventLevel.Information);
        }

        [Fact]
        public void GivenMultipleWarningMessagesAndAssertingInformation_WithLevelFails()
        {
            _logger.Warning("Hello, world!");
            _logger.Warning("Hello, world!");
            _logger.Warning("Hello, world!");

            Action action = () => InMemorySink
                .Instance
                .Should()
                .HaveMessage("Hello, world!")
                .Appearing().Times(3)
                .WithLevel(LogEventLevel.Information);

            Should.Throw<XunitException>(() => action())
                .Message
                .ShouldBe("Expected instances of log message \"Hello, world!\" to have level \"Information\", but found 3 with level \"Warning\"");
        }
    }
}