using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions;

public interface LogEventsPropertyAssertion : ISubjectAssertions<IReadOnlyCollection<LogEvent>>
{
    LogEventsAssertions WithValues(params object[] values);
}