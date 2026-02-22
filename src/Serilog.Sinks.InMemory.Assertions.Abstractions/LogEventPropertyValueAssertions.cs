using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions;

public interface LogEventPropertyValueAssertions: ISubjectAssertions<LogEventPropertyValue>
{
    TValue WhichValue<TValue>();
    StructuredValueAssertions HavingADestructuredObject(string because = "", params object[] becauseArgs);
    LogEventAssertion WithValue(object value, string because = "", params object[] becauseArgs);
}