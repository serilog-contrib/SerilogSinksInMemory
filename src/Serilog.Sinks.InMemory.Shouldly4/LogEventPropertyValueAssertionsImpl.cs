namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

partial class LogEventPropertyValueAssertionsImpl : BaseShouldlyAssertions<LogEventPropertyValue>
{
    public LogEventPropertyValueAssertionsImpl(
        LogEventAssertionImpl logEventAssertion,
        LogEventPropertyValue instance,
        string propertyName)
        : base(instance)
    {
        _logEventAssertion = logEventAssertion;
        _propertyName = propertyName;
    }
}