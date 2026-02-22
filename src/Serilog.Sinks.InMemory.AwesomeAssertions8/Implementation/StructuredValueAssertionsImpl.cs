namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

public partial class StructuredValueAssertionsImpl : StructuredValueAssertions
{
    private readonly string _propertyName;
    private readonly LogEventAssertionImpl _logEventAssertion;

    public LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs)
    {
        Assert(Subject.Properties.Any(p => p.Name == name),
            new("Expected destructured object property {0} to have a property {1} but it wasn't found",
                _propertyName,
                name),
            because, becauseArgs);

        return new LogEventPropertyValueAssertionsImpl(
            _logEventAssertion,
            Subject.Properties.Single(p => p.Name == name).Value,
            name);
    }
}