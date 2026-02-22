namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

public partial class LogEventAssertionImpl : LogEventAssertion, IInMemorySinkAssertionsExtension
{
    private readonly string _messageTemplate;

    public LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs)
    {
        if (name.StartsWith("@"))
        {
            name = name.Substring(1);
        }

        Assert(Subject.Properties.ContainsKey(name),
            new FailMessage("Expected message {0} to have a property {1} but it wasn't found", _messageTemplate, name),
            because, becauseArgs);

        var logEventPropertyValue = Subject.Properties[name];
        return this.CreateLogEventPropertyValueAssertions(name, logEventPropertyValue);
    }

    public LogEventAssertion WithLevel(LogEventLevel level, string because = "", params object[] becauseArgs)
    {
        Assert(Subject.Level == level,
            new("Expected message {0} to have level {1}, but it is {2}", _messageTemplate, level.ToString(), Subject.Level.ToString()),
            because, becauseArgs);

        return this;
    }
}