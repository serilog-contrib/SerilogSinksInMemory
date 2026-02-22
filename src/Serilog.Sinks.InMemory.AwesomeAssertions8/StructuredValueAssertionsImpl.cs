namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

partial class StructuredValueAssertionsImpl : BaseAssertions<StructureValue, StructuredValueAssertionsImpl>
{
    public StructuredValueAssertionsImpl(LogEventAssertionImpl logEventAssertion, StructureValue subject, string propertyName)
        : base(subject, logEventAssertion.CurrentAssertionChain)
    {
        _propertyName = propertyName;
        _logEventAssertion = logEventAssertion;
    }

    protected override string Identifier => _propertyName;
}