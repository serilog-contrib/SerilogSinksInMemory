using FluentAssertions;

namespace Serilog.Sinks.InMemory.Assertions;

public interface LogEventsPropertyAssertion
{
    AndConstraint<LogEventsAssertions> WithValues(params object[] values);
}