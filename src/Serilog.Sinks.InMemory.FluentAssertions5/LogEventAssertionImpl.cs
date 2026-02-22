namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

partial class LogEventAssertionImpl : BaseAssertions<LogEvent, LogEventAssertionImpl>
{
    public LogEventAssertionImpl(string messageTemplate, LogEvent subject)
        : base(subject)
    {
        _messageTemplate = messageTemplate;
    }

    protected override string Identifier => "log event";

    public void Match(Func<LogEvent, bool> predicate)
        => Subject.Should().Match<LogEvent>(o => predicate(o));
}