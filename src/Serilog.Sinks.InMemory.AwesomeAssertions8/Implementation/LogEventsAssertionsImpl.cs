namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

public partial class LogEventsAssertionsImpl : LogEventsAssertions
{
    private readonly string _messageTemplate;

    public LogEventsAssertions Appearing()
    {
        return this;
    }

    public LogEventAssertion Once(string because = "", params object[] becauseArgs)
    {
        Assert(Subject.Count == 1,
            new("Expected message {0} to appear exactly once, but it was found {1} times",
                _messageTemplate,
                Subject.Count),
            because, becauseArgs);

        return this.CreateLogEventAssertion(_messageTemplate, Subject.Single());
    }

    public LogEventsAssertions Times(int number, string because = "", params object[] becauseArgs)
    {
        Assert(Subject.Count == number,
            new("Expected message {0} to appear {1} times, but it was found {2} times",
                _messageTemplate,
                number,
                Subject.Count),
            because, becauseArgs);

        return this;
    }

    public LogEventsAssertions WithLevel(LogEventLevel level, string because = "", params object[] becauseArgs)
    {
        var notMatched = Subject.Where(logEvent => logEvent.Level != level).ToList();

        var notMatchedText = "";

        if (notMatched.Any())
        {
            notMatchedText = string.Join(" and ",
                notMatched
                    .GroupBy(logEvent => logEvent.Level,
                        logEvent => logEvent,
                        (key, values) => $"{values.Count()} with level \"{key}\""));
        }

        Assert(Subject.All(logEvent => logEvent.Level == level),
            new($"Expected instances of log message {{0}} to have level {{1}}, but found {notMatchedText}",
                _messageTemplate,
                level.ToString()),
            because, becauseArgs);

        return this;
    }

    public LogEventsPropertyAssertion WithProperty(string propertyName, string because = "", params object[] becauseArgs)
    {
        Assert(Subject.All(logEvent => logEvent.Properties.ContainsKey(propertyName)),
            new("Expected all instances of log message {0} to have property {1}, but it was not found",
                _messageTemplate,
                propertyName),
            because, becauseArgs);

        return this.CreateLogEventsPropertyAssertion(propertyName);
    }
}