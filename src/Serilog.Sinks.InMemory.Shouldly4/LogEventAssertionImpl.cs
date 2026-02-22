namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

[ShouldlyMethods]
partial class LogEventAssertionImpl : BaseShouldlyAssertions<LogEvent>
{
    public LogEventAssertionImpl(string messageTemplate, LogEvent subject)
        : base(subject)
    {
        _messageTemplate = messageTemplate;
    }

    public void Match(Func<LogEvent, bool> predicate)
        => Subject.ShouldSatisfyAllConditions(logEvent => predicate(logEvent).ShouldBeTrue());
}