using FluentAssertions;

namespace Serilog.Sinks.InMemory.Assertions;

public interface LogEventsPropertyAssertion
{
    LogEventsAssertions WithValues(params object[] values);
}