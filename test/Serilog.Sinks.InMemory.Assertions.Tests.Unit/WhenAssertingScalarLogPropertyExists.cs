using System;
using FluentAssertions;
using Serilog.Context;
using Xunit;
using Xunit.Sdk;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingScalarLogPropertyExists
    {
        private readonly ILogger _logger;

        public WhenAssertingScalarLogPropertyExists()
        {
            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.InMemory()
                .CreateLogger();
        }

        [Fact]
        public void GivenMessageIsLoggedWithProperty_HavePropertySucceeds()
        {
            _logger.Information("Hello {name}", "World");

            InMemorySink.Instance
                .Should()
                .HaveMessage("Hello {name}")
                .Appearing().Once()
                .WithProperty("name");
        }

        [Fact]
        public void GivenMessageIsLoggedWithoutProperty_HavePropertyFails()
        {
            _logger.Information("Hello {name}", "World");

            Action action = () => InMemorySink.Instance
                .Should()
                .HaveMessage("Hello {name}")
                .Appearing().Once()
                .WithProperty("something else");

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected message \"Hello {name}\" to have a property \"something else\" but it wasn't found");
        }

        [Fact]
        public void GivenMessageIsLoggedWithPropertyAndAssertingValue_HavePropertySucceeds()
        {
            _logger.Information("Hello {name}", "World");

            InMemorySink.Instance
                .Should()
                .HaveMessage("Hello {name}")
                .Appearing().Once()
                .WithProperty("name")
                .WithValue("World");
        }

        [Fact]
        public void GivenMessageIsLoggedWithPropertyAndAssertingDifferentValue_HavePropertyFails()
        {
            _logger.Information("Hello {name}", "World");

            Action action = () => InMemorySink.Instance
                .Should()
                .HaveMessage("Hello {name}")
                .Appearing().Once()
                .WithProperty("name")
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

            InMemorySink.Instance
                .Should()
                .HaveMessage("Message number {number}")
                .Appearing().Once()
                .WithProperty("number")
                .WithValue(5);
        }

        [Fact]
        public void GivenMessageIsLoggedWithIntegerPropertyAndAssertingDifferentValue_HavePropertyFails()
        {
            _logger.Information("Message number {number}", 5);

            Action action = () => InMemorySink.Instance
                .Should()
                .HaveMessage("Message number {number}")
                .Appearing().Once()
                .WithProperty("number")
                .WithValue(2);

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected property \"number\" to have value 2 but found 5");
        }

        [Fact]
        public void GivenMessageIsLoggedWithPropertyFromContext_HavePropertySucceeds()
        {
            using (LogContext.PushProperty("some_property", "some_value"))
            {
                _logger.Information("Message number {number}", 5);
            }

            InMemorySink.Instance
                .Should()
                .HaveMessage("Message number {number}")
                .Appearing().Once()
                .WithProperty("some_property")
                .WithValue("some_value");
        }

        [Fact]
        public void GivenLogMessageWithTwoProperties_BothPropertiesExistOnLogEntry()
        {
            _logger.Information("{PropertyOne} {PropertyTwo}", "one", "two");

            InMemorySink.Instance
                .Should()
                .HaveMessage()
                .Appearing().Once()
                .WithProperty("PropertyOne")
                .WithValue("one")
                .And
                .WithProperty("PropertyTwo")
                .WithValue("two");
        }
    }
}