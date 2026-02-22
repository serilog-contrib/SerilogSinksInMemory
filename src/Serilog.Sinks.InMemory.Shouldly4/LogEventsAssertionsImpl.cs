namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

[ShouldlyMethods]
partial class LogEventsAssertionsImpl : BaseShouldlyAssertions<IReadOnlyCollection<LogEvent>>
{
    public LogEventsAssertionsImpl(string messageTemplate, IReadOnlyCollection<LogEvent> logEvents)
        : base(logEvents)
    {
        _messageTemplate = messageTemplate;
    }
}