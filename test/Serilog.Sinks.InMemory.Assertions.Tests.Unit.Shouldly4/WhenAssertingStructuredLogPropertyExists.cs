using System;
using Shouldly;
using Xunit;
using Xunit.Sdk;
using Serilog.Sinks.InMemory.Assertions;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingStructuredLogPropertyExists
    {
        private readonly ILogger _logger;
        private readonly InMemorySink _inMemorySink;

        public WhenAssertingStructuredLogPropertyExists()
        {
            _inMemorySink = new InMemorySink();

            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Sink(_inMemorySink)
                .CreateLogger();

            _logger.Information("Hello {@Placeholder}", new PlaceholderObject());
        }

        public class PlaceholderObject
        {
            public string Name { get; set; } = "Joe Blogs";
        }

        [Fact]
        public void GivenMessageIsLoggedWithDestructuredObjectAndAssertionPropertyNameSpecifiedWithoutAt_HavePropertySucceeds()
        {
            _inMemorySink
                .Should()
                .HaveMessage("Hello {@Placeholder}")
                .Appearing().Once()
                .WithProperty("Placeholder");
        }

        [Fact]
        public void GivenMessageIsLoggedWithDestructuredObjectAndAssertionPropertyNameSpecifiedWithAt_HavePropertySucceeds()
        {
            _inMemorySink
                .Should()
                .HaveMessage("Hello {@Placeholder}")
                .Appearing().Once()
                .WithProperty("@Placeholder");
        }

        [Fact]
        public void GivenMessageisLoggedWithDestructuredObjectAndAssertingPropertyValue_WithValueSucceeds()
        {
            _inMemorySink
                .Should()
                .HaveMessage("Hello {@Placeholder}")
                .Appearing().Once()
                .WithProperty("Placeholder")
                .HavingADestructuredObject()
                .WithProperty("Name")
                .WithValue("Joe Blogs");
        }

        [Fact]
        public void GivenMessageisLoggedWithRightPropertyButValueIsNotADestructuredObject_AssertionFails()
        {
            _logger.Information("Hello {NotDestructured}", "scalar value");

            Action action = () => _inMemorySink
                .Should()
                .HaveMessage("Hello {NotDestructured}")
                .Appearing().Once()
                .WithProperty("NotDestructured")
                .HavingADestructuredObject()
                .WithProperty("Name")
                .WithValue("Joe Blogs");

            Should.Throw<Exception>(() => action())
                .Message
                .ShouldBe("Expected message \"Hello {NotDestructured}\" to have a property \"NotDestructured\" that holds a destructured object but found a scalar value");
        }
    }
}