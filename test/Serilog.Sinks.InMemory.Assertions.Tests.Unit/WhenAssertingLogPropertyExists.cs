using System;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingLogPropertyExists
    {
        private readonly InMemorySink _sink;
        private readonly ILogger _logger;

        public WhenAssertingLogPropertyExists()
        {
            _sink = new InMemorySink();
            _logger = new LoggerConfiguration()
                .WriteTo.Sink(_sink)
                .CreateLogger();
        }

        [Fact]
        public void GivenMessageIsLoggedWithProperty_Have()
        {
            _logger.Information("Hello {name}", "World");

            _sink
                .Should()
                .HaveMessage("Hello {name}")
                .And
                .AppearsOnce()
                .Which
                .Should()
                .HaveProperty("name");
        }

        [Fact]
        public void GivenMessageIsLoggedWithoutProperty_HavePropertyFails()
        {
            _logger.Information("Hello {name}", "World");

            Action action = () => _sink
                .Should()
                .HaveMessage("Hello {name}")
                .And
                .AppearsOnce()
                .Which
                .Should()
                .HaveProperty("something else");

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected log message to have a property \"something else\" but it wasn't found");
        }
    }
}