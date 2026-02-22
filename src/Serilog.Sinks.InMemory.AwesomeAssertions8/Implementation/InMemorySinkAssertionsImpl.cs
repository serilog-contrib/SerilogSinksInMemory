#nullable enable

namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

public partial class InMemorySinkAssertionsImpl : InMemorySinkAssertions
{
    public LogEventsAssertions HaveMessage(
        Func<LogEvent, bool> predicate,
        string? predicateErrorName = null,
        string because = "",
        params object[] becauseArgs)
    {
        predicateErrorName ??= "<predicate>";

        var matches = Subject
            .LogEvents
            .Where(predicate)
            .ToList();

        Assert(matches.Any(), new("Expected message {0} to be logged", predicateErrorName), because, becauseArgs);

        return this.CreateLogEventsAssertions(predicateErrorName, matches);
    }

    public LogEventsAssertions HaveMessage(
        string messageTemplate,
        string because = "",
        params object[] becauseArgs)
    {
        return HaveMessage(logEvent => logEvent.MessageTemplate.Text == messageTemplate, messageTemplate, because, becauseArgs);
    }

    public PatternLogEventsAssertions HaveMessage()
    {
        var logEvents = Subject.LogEvents;
        return this.CreatePatternLogEventsAssertions(logEvents);
    }

    public void NotHaveMessage(
        string? messageTemplate = null,
        string because = "",
        params object[] becauseArgs)
    {
        if (messageTemplate != null)
        {
            NotHaveMessage(logEvent => logEvent.MessageTemplate.Text == messageTemplate, messageTemplate, because, becauseArgs);
        }
        else
        {
            NotHaveMessage(null, messageTemplate, because, becauseArgs);
        }
    }

    public void NotHaveMessage(Func<LogEvent, bool>? predicate, string? predicateErrorName = null, string because = "", params object[] becauseArgs)
    {
        predicateErrorName ??= "<predicate>";

        int count;
        string failureMessage;

        if (predicate != null)
        {
            count = Subject.LogEvents.Count(predicate);

            failureMessage = $"Expected message \"{predicateErrorName}\" not to be logged, but it was found {(count > 1 ? $"{count} times" : "once")}";
        }
        else
        {
            count = Subject.LogEvents.Count();

            failureMessage = $"Expected no messages to be logged, but found {(count > 1 ? $"{count} messages" : "message")}";
        }

        Assert(count == 0, failureMessage, because, becauseArgs);
    }
}