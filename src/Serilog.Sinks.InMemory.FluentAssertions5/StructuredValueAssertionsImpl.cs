using System.Linq;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;

namespace Serilog.Sinks.InMemory.FluentAssertions5;

public class StructuredValueAssertionsImpl : ReferenceTypeAssertions<StructureValue, StructuredValueAssertionsImpl>, StructuredValueAssertions
{
    private readonly LogEventAssertionImpl _logEventAssertion;

    public StructuredValueAssertionsImpl(StructureValue subject, string propertyName, LogEventAssertionImpl logEventAssertion) : base(subject)
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

        return new LogEventPropertyValueAssertionsImpl(
            _logEventAssertion,
            Subject.Properties.Single(p => p.Name == name).Value,
            name);
    }
}