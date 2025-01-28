namespace Serilog.Sinks.InMemory.Assertions.Abstractions
{
    public interface InMemorySinkAssertions
    {
        LogEventsAssertions HaveMessage(
            string messageTemplate,
            string because = "",
            params object[] becauseArgs);

        PatternLogEventsAssertions HaveMessage();

        void NotHaveMessage(
            string messageTemplate = null,
            string because = "",
            params object[] becauseArgs);
    }
}