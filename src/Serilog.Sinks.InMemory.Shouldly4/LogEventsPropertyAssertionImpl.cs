namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

[ShouldlyMethods]
partial class LogEventsPropertyAssertionImpl : BaseShouldlyAssertions<IReadOnlyCollection<LogEvent>>
{
    public LogEventsPropertyAssertionImpl(LogEventsAssertionsImpl logEventsAssertions, string propertyName)
        : base(logEventsAssertions.Subject)
    {
        _logEventsAssertions = logEventsAssertions;
        _propertyName = propertyName;
    }
}