using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions;

public class LogEventPropertyValueAssertions : ReferenceTypeAssertions<LogEventPropertyValue, LogEventPropertyValueAssertions>
{
    private readonly LogEventAssertion _logEventAssertion;

    public LogEventPropertyValueAssertions(LogEventAssertion logEventAssertion, LogEventPropertyValue instance, string propertyName)
        : base(instance)
    {
        _logEventAssertion = logEventAssertion;
        Identifier = propertyName;
    }

    protected override string Identifier { get; }

    public TValue WhichValue<TValue>()
    {
        if (Subject is ScalarValue scalarValue)
        {
            Execute.Assertion
                .ForCondition(scalarValue.Value is TValue)
                .FailWith("Expected property value to be of type {0} but found {1}",
                    typeof(TValue).Name,
                    scalarValue.Value.GetType().Name);

            return (TValue)scalarValue.Value;
        }

        throw new Exception($"Expected property value to be of type {typeof(TValue).Name} but the property value is not a scalar and I don't know how to handle that");
    }

    public AndConstraint<LogEventAssertion> WithValue(object value, string because = "", params object[] becauseArgs)
    {
        var actualValue = GetValueFromProperty(Subject);

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Equals(actualValue, value))
            .FailWith("Expected property {0} to have value {1} but found {2}",
                Identifier,
                value,
                actualValue);

        return new AndConstraint<LogEventAssertion>(_logEventAssertion);
    }

    private object GetValueFromProperty(LogEventPropertyValue instance)
    {
        return instance switch
        {
            ScalarValue scalarValue => scalarValue.Value,
            _ => Subject.ToString(),
        };
    }

    public StructuredValueAssertions HavingADestructuredObject(string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject is StructureValue)
            .FailWith("Expected message \"{0}\" to have a property {1} that holds a destructured object but found a scalar value",
                _logEventAssertion.Subject.MessageTemplate,
                Identifier);

        return new StructuredValueAssertions(Subject as StructureValue, Identifier, _logEventAssertion);
    }
}

public class StructuredValueAssertions : ReferenceTypeAssertions<StructureValue, StructuredValueAssertions>
{
    private readonly LogEventAssertion _logEventAssertion;

    public StructuredValueAssertions(StructureValue subject, string propertyName, LogEventAssertion logEventAssertion) : base(subject)
    {
        Identifier = propertyName;
        _logEventAssertion = logEventAssertion;
    }

    protected override string Identifier { get; }

    public LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Properties.Any(p => p.Name == name))
            .FailWith("Expected destructured object property {0} to have a property {1} but it wasn't found",
                Identifier,
                name);

        return new LogEventPropertyValueAssertions(
            _logEventAssertion,
            Subject.Properties.Single(p => p.Name == name).Value,
            name);
    }
}
