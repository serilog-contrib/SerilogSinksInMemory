namespace Serilog.Sinks.InMemory.Assertions;

public interface StructuredValueAssertions
{
    LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs);
}