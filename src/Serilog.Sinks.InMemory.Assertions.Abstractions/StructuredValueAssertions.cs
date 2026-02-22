using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions;

public interface StructuredValueAssertions : ISubjectAssertions<StructureValue>
{
    LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs);
}