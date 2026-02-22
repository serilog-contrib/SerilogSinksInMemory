namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

partial class LogEventAssertionImpl : BaseAssertions<LogEvent, LogEventAssertionImpl>
{
    public LogEventAssertionImpl(string messageTemplate, LogEvent subject, AssertionChain assertionChain)
        : base(subject, assertionChain)
    {
        _messageTemplate = messageTemplate;
    }

    protected override string Identifier => "log event";

    public void Match(Func<LogEvent, bool> predicate)
        => Subject.Should().Match<LogEvent>(o => predicate(o));
}