namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

public partial class PatternLogEventsAssertionsImpl : LogEventsAssertionsImpl, PatternLogEventsAssertions
{
    public LogEventsAssertions Containing(
        string pattern,
        string because = "",
        params object[] becauseArgs)
    {
        var matches = Subject
            .Where(logEvent => logEvent.MessageTemplate.Text.Contains(pattern))
            .ToArray();

        Assert(matches.Any(), new("Expected a message with pattern {0} to be logged", pattern), because, becauseArgs);

        return this.CreateLogEventsAssertions(pattern, matches);
    }
}