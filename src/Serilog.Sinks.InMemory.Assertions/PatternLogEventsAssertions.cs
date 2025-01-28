namespace Serilog.Sinks.InMemory.Assertions;

public interface PatternLogEventsAssertions : LogEventsAssertions
{
    LogEventsAssertions Containing(
        string pattern,
        string because = "",
        params object[] becauseArgs);
}