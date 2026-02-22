namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

partial class LogEventPropertyValueAssertionsImpl : BaseAssertions<LogEventPropertyValue, LogEventPropertyValueAssertionsImpl>
{
    public LogEventPropertyValueAssertionsImpl(LogEventAssertionImpl logEventAssertion, LogEventPropertyValue instance, string propertyName)
        : base(instance, logEventAssertion.CurrentAssertionChain)
    {
        _logEventAssertion = logEventAssertion;
        _propertyName = propertyName;
    }

    protected override string Identifier => _propertyName;
}