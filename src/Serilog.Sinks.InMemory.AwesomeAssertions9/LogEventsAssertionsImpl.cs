namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

partial class LogEventsAssertionsImpl : BaseAssertions<IReadOnlyCollection<LogEvent>, LogEventsAssertionsImpl>
{
    public LogEventsAssertionsImpl(string messageTemplate, IReadOnlyCollection<LogEvent> matches, AssertionChain assertionChain)
        : base(matches, assertionChain)
    {
        _messageTemplate = messageTemplate;
    }

    protected override string Identifier => "log events";
}