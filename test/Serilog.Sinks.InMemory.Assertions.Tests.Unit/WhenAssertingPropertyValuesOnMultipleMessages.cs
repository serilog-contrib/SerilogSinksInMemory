namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit;

public class WhenAssertingPropertyValuesOnMultipleMessages
{
    [Fact]
    public void GivenMessageDoesNotContainProperty_AssertionFails()
    {
        _logger.Information("Message without property");

        Action action = () => InMemorySink.Instance
            .Should()
            .HaveMessage("Message without property")
            .Appearing().Times(1)
            .WithProperty("SomeProperty");

        action
            .Should()
            .Throw<Exception>()
            .Which
            .Message
            .Should()
            .Be("Expected all instances of log message \"Message without property\" to have property \"SomeProperty\", but it was not found");
    }

    [Fact]
    public void GivenOneMessageHasNullPropertyValue_AssertionFails()
    {
        _logger.Information("Message with {Property}", "val1");
        _logger.Information("Message with {Property}", null);

        Action action = () => InMemorySink.Instance
            .Should()
            .HaveMessage("Message with {Property}")
            .Appearing().Times(2)
            .WithProperty("Property")
            .WithValues("val1", "val1");

        action
            .Should()
            .Throw<Exception>()
            .Which
            .Message
            .Should()
            .Be("Expected all instances of log message \"Message with {Property}\" to have property \"Property\", but it was not found");
    }

    [Fact]
    public void GivenMessageAppearsTwiceWithUniquePropertyValues_AssertionForBothValuesPasses()
    {
        _logger.Information("Message with {Property}", "val1");
        _logger.Information("Message with {Property}", "val2");

        InMemorySink.Instance
            .Should()
            .HaveMessage("Message with {Property}")
            .Appearing().Times(2)
            .WithProperty("Property")
            .WithValues("val1", "val2");
    }

    [Fact]
    public void GivenMessageAppearsTwiceAndAssertingOnThreeValues_AssertionFailsWithNumberOfValuesError()
    {
        _logger.Information("Message with {Property}", "val1");
        _logger.Information("Message with {Property}", "val2");

        Action action = () => InMemorySink.Instance
            .Should()
            .HaveMessage("Message with {Property}")
            .Appearing().Times(2)
            .WithProperty("Property")
            .WithValues("val1", "val2", "val3");

        action
            .Should()
            .Throw<Exception>()
            .Which
            .Message
            .Should()
            .Be("Can't assert property values because 3 values were provided while only 2 messages were expected");
    }

    [Fact]
    public void GivenMessageAppearsTwiceWithSameValueAndAssertingOnOtherValue_AssertionFails()
    {
        _logger.Information("Message with {Property}", "val1");
        _logger.Information("Message with {Property}", "val1");

        Action action = () => InMemorySink.Instance
            .Should()
            .HaveMessage("Message with {Property}")
            .Appearing().Times(2)
            .WithProperty("Property")
            .WithValues("val1", "other");

        action
            .Should()
            .Throw<Exception>()
            .Which
            .Message
            .Should()
            .Be("Expected property values {\"val1\", \"val1\"} to contain {\"val1\", \"other\"} but did not find {\"other\"}");
    }

    [Fact]
    public void AssertionShouldFailWithMissingValueEvenIfValuesAreInDifferentOrder()
    {
        _logger.Information("Message with {Property}", "val1");
        _logger.Information("Message with {Property}", "val1");

        Action action = () => InMemorySink.Instance
            .Should()
            .HaveMessage("Message with {Property}")
            .Appearing().Times(2)
            .WithProperty("Property")
            .WithValues("other", "val1");

        action
            .Should()
            .Throw<Exception>()
            .Which
            .Message
            .Should()
            .Be("Expected property values {\"val1\", \"val1\"} to contain {\"other\", \"val1\"} but did not find {\"other\"}");
    }

    [Fact]
    public void GivenMessageWithTwoPropertiesAndAssertingOnBothProperties_AssertionPasses()
    {
        _logger.Information("Message with {Property1} and {Property2}", "val1", "valA");
        _logger.Information("Message with {Property1} and {Property2}", "val2", "valB");

        InMemorySink.Instance
            .Should()
            .HaveMessage("Message with {Property1} and {Property2}")
            .Appearing().Times(2)
            .WithProperty("Property1")
            .WithValues("val1", "val2")
            .And
            .WithProperty("Property2")
            .WithValues("valA", "valB");
    }

    [Fact]
    public void GivenMessageWithTwoPropertiesAndAssertingOnBothPropertiesWithOneValueNotPresentOnSecondProperty_AssertionFails()
    {
        _logger.Information("Message with {Property1} and {Property2}", "val1", "valA");
        _logger.Information("Message with {Property1} and {Property2}", "val2", "XXX");

        Action action = () => InMemorySink.Instance
            .Should()
            .HaveMessage("Message with {Property1} and {Property2}")
            .Appearing().Times(2)
            .WithProperty("Property1")
            .WithValues("val1", "val2")
            .And
            .WithProperty("Property2")
            .WithValues("valA", "valB");

        action
            .Should()
            .Throw<Exception>()
            .Which
            .Message
            .Should()
            .Be("Expected property values {\"valA\", \"XXX\"} to contain {\"valA\", \"valB\"} but did not find {\"valB\"}");
    }

    private readonly ILogger _logger;

    public WhenAssertingPropertyValuesOnMultipleMessages()
    {
        _logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.InMemory()
            .CreateLogger();
    }
}
