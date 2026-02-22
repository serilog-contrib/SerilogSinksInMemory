namespace Serilog.Sinks.InMemory.AssertionsTests;

public class WhenAssertingWithPredicate
{
    private readonly ILogger _logger;

    public WhenAssertingWithPredicate()
    {
        _logger = new LoggerConfiguration()
            .WriteTo.InMemory()
            .CreateLogger();
    }

    [Fact]
    public void GivenMatchingMessage_HaveMessageWithPredicateSucceeds()
    {
        _logger.Information("Error code 404");
        _logger.Information("Error code 500");

        InMemorySink.Instance
            .Should()
            .HaveMessage(
                logEvent => logEvent.MessageTemplate.Text.Contains("404"),
                "message containing '404'")
            .Appearing().Once();
    }

    [Fact]
    public void GivenNoMatchingMessage_HaveMessageWithPredicateFails()
    {
        _logger.Information("Error code 200");

        Action action = () => InMemorySink.Instance
            .Should()
            .HaveMessage(
                logEvent => logEvent.MessageTemplate.Text.Contains("404"),
                "message containing '404'");

        action
            .Should()
            .Throw<XunitException>()
            .WithMessage("*message containing '404'*");
    }

    [Fact]
    public void GivenMatchingMessage_NotHaveMessageWithPredicateFails()
    {
        _logger.Information("Secret value 12345");

        Action action = () => InMemorySink.Instance
            .Should()
            .NotHaveMessage(
                logEvent => logEvent.MessageTemplate.Text.Contains("Secret"),
                "message containing 'Secret'");

        action
            .Should()
            .Throw<XunitException>();
    }

    [Fact]
    public void GivenNoMatchingMessage_NotHaveMessageWithPredicateSucceeds()
    {
        _logger.Information("Public info only");

        InMemorySink.Instance
            .Should()
            .NotHaveMessage(
                logEvent => logEvent.MessageTemplate.Text.Contains("Secret"),
                "message containing 'Secret'");
    }
}
