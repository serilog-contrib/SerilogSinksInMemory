using FluentAssertions.Execution;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions;

public class PatternLogEventsAssertions : LogEventsAssertions
{
    public PatternLogEventsAssertions(IEnumerable<LogEvent> subjectLogEvents) : base(null, subjectLogEvents)
    {
    }

    public LogEventsAssertions Containing(
        string pattern,
        string because = "",
        params object[] becauseArgs)
    {
        var matches = Subject
            .Where(logEvent => logEvent.MessageTemplate.Text.Contains(pattern))
            .ToList();

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(matches.Any())
            .FailWith(
                "Expected a message with pattern {0} to be logged",
                pattern);

        return new LogEventsAssertions(pattern, matches);
    }
}
