namespace Serilog.Sinks.InMemory.Tests.Unit;

public class WhenLoggingToInMemorySink
{
    [Fact]
    public void GivenInformationMessageIsWritten_LogEventIsStoredInSink()
    {
        var logger = new LoggerConfiguration()
            .WriteTo.InMemory()
            .CreateLogger();

        logger.Information("Test");

        InMemorySink.Instance
            .LogEvents
            .Should()
            .HaveCount(1);
    }

    [Fact]
    public void GivenRestrictedMinimumLevelMessagesAreWritten_FilteredLogEventsAreStoredInSink()
    {
        var logger = new LoggerConfiguration()
            .WriteTo.InMemory(restrictedToMinimumLevel: Events.LogEventLevel.Information)
            .CreateLogger();

        logger.Verbose("Verbose Message");
        logger.Debug("Debug Message");
        logger.Information("Information Message");

        InMemorySink.Instance
            .LogEvents
            .Should()
            .HaveCount(1);
    }

    [Fact]
    public void GivenLoggerIsDisposed_LogEventsAreCleared()
    {
        var logger = new LoggerConfiguration()
            .WriteTo.InMemory()
            .CreateLogger();

        logger.Information("Test");

        logger.Dispose();

        InMemorySink.Instance
            .LogEvents
            .Should()
            .HaveCount(0);
    }

    [Fact]
    public void GivenLoggerIsDisposedAndNewMessageIsLogged_SinkOnlyContainsSecondMessage()
    {
        var logger = new LoggerConfiguration()
            .WriteTo.InMemory()
            .CreateLogger();

        logger.Information("First");

        logger.Dispose();

        logger.Information("Second");

        InMemorySink.Instance
            .LogEvents
            .Should()
            .OnlyContain(l => l.MessageTemplate.Text == "Second");
    }
}
