namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

partial class StructuredValueAssertionsImpl : BaseShouldlyAssertions<StructureValue>
{
    public StructuredValueAssertionsImpl(LogEventAssertionImpl logEventAssertion, StructureValue subject, string propertyName)
        : base(subject)
    {
        _propertyName = propertyName;
        _logEventAssertion = logEventAssertion;
    }
}