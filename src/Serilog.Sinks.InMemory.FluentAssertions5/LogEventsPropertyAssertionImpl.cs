namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

partial class LogEventsPropertyAssertionImpl : BaseAssertions<IReadOnlyCollection<LogEvent>, LogEventsPropertyAssertionImpl>
{
    public LogEventsPropertyAssertionImpl(LogEventsAssertionsImpl logEventsAssertions, string propertyName)
        : base(logEventsAssertions.Subject)
    {
        _logEventsAssertions = logEventsAssertions;
        _propertyName = propertyName;
    }

    protected override string Identifier => _propertyName;
}