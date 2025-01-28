namespace Serilog.Sinks.InMemory.Assertions.Abstractions;

public interface PatternLogEventsAssertions : LogEventsAssertions
{
    LogEventsAssertions Containing(
        string pattern,
        string because = "",
        params object[] becauseArgs);
}