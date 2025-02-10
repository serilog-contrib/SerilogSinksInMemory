using System.Linq;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;
using Shouldly;

namespace Serilog.Sinks.InMemory.Shouldly4;

public class StructuredValueAssertionsImpl : StructuredValueAssertions
{
    private readonly StructureValue _subject;
    private readonly string _propertyName;
    private readonly LogEventAssertionImpl _logEventAssertion;

    public StructuredValueAssertionsImpl(StructureValue subject, string propertyName, LogEventAssertionImpl logEventAssertion)
    {
        _subject = subject;
        _propertyName = propertyName;
        _logEventAssertion = logEventAssertion;
    }

    public LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs)
    {
        _subject
            .Properties
            .ShouldContain(p => p.Name == _propertyName,
                $"Expected destructured object property '{_propertyName}' to have a property '{name}' but it wasn't found");

        return new LogEventPropertyValueAssertionsImpl(
            _logEventAssertion,
            _subject.Properties.Single(p => p.Name == name).Value,
            name);
    }
}
