namespace Serilog.Sinks.InMemory.Assertions.Abstractions;

public interface StructuredValueAssertions
{
    LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs);
}