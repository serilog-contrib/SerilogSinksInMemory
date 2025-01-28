using FluentAssertions;

namespace Serilog.Sinks.InMemory.Assertions.Abstractions;

public interface LogEventPropertyValueAssertions
{
    TValue WhichValue<TValue>();
    //AndConstraint<LogEventAssertion> WithValue(object value, string because = "", params object[] becauseArgs);
    StructuredValueAssertions HavingADestructuredObject(string because = "", params object[] becauseArgs);
    AndConstraint<LogEventAssertion> WithValue(object value, string because = "", params object[] becauseArgs);
}