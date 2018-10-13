using System;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingScalarLogPropertyExists
    {
        private readonly InMemorySink _sink;
        private readonly ILogger _logger;

        public WhenAssertingScalarLogPropertyExists()
        {
            _sink = new InMemorySink();
            _logger = new LoggerConfiguration()
                .WriteTo.Sink(_sink)
                .CreateLogger();
        }

        [Fact]
        public void GivenMessageIsLoggedWithProperty_HavePropertySucceeds()
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

        [Fact]
        public void GivenMessageIsLoggedWithPropertyAndAssertingValue_HavePropertySucceeds()
        {
            _logger.Information("Hello {name}", "World");

            _sink
                .Should()
                .HaveMessage("Hello {name}")
                .And
                .AppearsOnce()
                .Which
                .Should()
                .HaveProperty("name")
                .WithValue("World");
        }

        [Fact]
        public void GivenMessageIsLoggedWithPropertyAndAssertingDifferentValue_HavePropertyFails()
        {
            _logger.Information("Hello {name}", "World");

            Action action = () => _sink
                .Should()
                .HaveMessage("Hello {name}")
                .And
                .AppearsOnce()
                .Which
                .Should()
                .HaveProperty("name")
                .WithValue("BLABLABLA");

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected property \"name\" to have value \"BLABLABLA\" but found \"World\"");
        }

        [Fact]
        public void GivenMessageIsLoggedWithIntegerPropertyValue_HavePropertySucceeds()
        {
            _logger.Information("Message number {number}", 5);

            _sink
                .Should()
                .HaveMessage("Message number {number}")
                .And
                .AppearsOnce()
                .Which
                .Should()
                .HaveProperty("number")
                .WithValue(5);
        }

        [Fact]
        public void GivenMessageIsLoggedWithIntegerPropertyAndAssertingDifferentValue_HavePropertyFails()
        {
            _logger.Information("Message number {number}", 5);

            Action action = () => _sink
                .Should()
                .HaveMessage("Message number {number}")
                .And
                .AppearsOnce()
                .Which
                .Should()
                .HaveProperty("number")
                .WithValue(2);

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected property \"number\" to have value 2 but found 5");
        }
    }
}