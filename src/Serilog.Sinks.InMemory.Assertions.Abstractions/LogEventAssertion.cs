using System;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions;

public interface LogEventAssertion : ISubjectAssertions<LogEvent>
{
    LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs);
    LogEventAssertion WithLevel(LogEventLevel level, string because = "", params object[] becauseArgs);
    void Match(Func<LogEvent, bool> predicate);
}