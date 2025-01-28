using FluentAssertions;

namespace Serilog.Sinks.InMemory.Assertions.Abstractions;

public interface LogEventsPropertyAssertion
{
    AndConstraint<LogEventsAssertions> WithValues(params object[] values);
}