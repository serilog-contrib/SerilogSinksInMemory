using System;
using FluentAssertions;
using NUnit.Framework;
using Serilog.Context;
using Serilog.Core;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingScalarLogPropertyExists
    {
        private readonly Logger _logger;

        public WhenAssertingScalarLogPropertyExists()
        {
            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
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
        public void GivenMessageIsLoggedWithProperty_HavePropertySucceeds()
        {
            _logger.Information("Hello {name}", "World");

            InMemorySink.Instance
                .Should()
                .HaveMessage("Hello {name}")
                .Appearing().Once()
                .WithProperty("name");
        }

        [Test]
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
                .Throw<Exception>()
                .WithMessage("Expected message \"Hello {name}\" to have a property \"something else\" but it wasn't found");
        }

        [Test]
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

        [Test]
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
                .Throw<Exception>()
                .WithMessage("Expected property \"name\" to have value \"BLABLABLA\" but found \"World\"");
        }

        [Test]
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

        [Test]
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
                .Throw<Exception>()
                .WithMessage("Expected property \"number\" to have value 2 but found 5");
        }

        [Test]
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
    }
}