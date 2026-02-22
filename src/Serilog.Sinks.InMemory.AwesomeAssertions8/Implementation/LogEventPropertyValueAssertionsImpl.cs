namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

public partial class LogEventPropertyValueAssertionsImpl : LogEventPropertyValueAssertions
{
    private readonly string _propertyName;
    private readonly LogEventAssertionImpl _logEventAssertion;

    public TValue WhichValue<TValue>()
    {
        if (Subject is ScalarValue scalarValue)
        {
            Assert(scalarValue.Value is TValue,
                new("Expected property value to be of type {0} but found {1}",
                    typeof(TValue).Name,
                    scalarValue.Value?.GetType().Name));

            return (TValue)scalarValue.Value;
        }

        throw new Exception(
            $"Expected property value to be of type {typeof(TValue).Name} but the property value is not a scalar and I don't know how to handle that");
    }

    public LogEventAssertion WithValue(object value, string because = "", params object[] becauseArgs)
    {
        var actualValue = GetValueFromProperty(Subject);

        Assert(Equals(actualValue, value),
            new("Expected property {0} to have value {1} but found {2}",
                _propertyName,
                value,
                actualValue),
            because, becauseArgs);

        return _logEventAssertion;
    }

    private object GetValueFromProperty(LogEventPropertyValue instance)
        => instance switch
        {
            ScalarValue scalarValue => scalarValue.Value,
            _ => Subject.ToString(),
        };

    public StructuredValueAssertions HavingADestructuredObject(string because = "", params object[] becauseArgs)
    {
        Assert(Subject is StructureValue,
            new("Expected message \"{0}\" to have a property {1} that holds a destructured object but found a scalar value",
                _logEventAssertion.Subject.MessageTemplate,
                _propertyName),
            because, becauseArgs);

        return _logEventAssertion.CreateStructuredValueAssertions(Subject as StructureValue, _propertyName);
    }
}