namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

internal static class AssertionExtensions
{
    public static readonly AssertionFramework AssertionFramework =
        new(AssertionFrameworks.FluentAssertions, new Version(8, 0));

    public static void Assert(
        this AssertionChain assertionChain,
        bool condition,
        FailMessage failureMessage,
        string because = "",
        params object[] becauseArgs)
    {
        assertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(condition)
            .FailWith(failureMessage.Message, failureMessage.Args);
    }

    public static LogEventsAssertions CreateLogEventsAssertions<TSubject, TAssertions>(
        this ReferenceTypeAssertions<TSubject, TAssertions> parent,
        string pattern,
        IReadOnlyCollection<LogEvent> logEvents)
        where TAssertions : BaseAssertions<TSubject, TAssertions>
        => new LogEventsAssertionsImpl(pattern, logEvents, parent.CurrentAssertionChain);

    public static PatternLogEventsAssertions CreatePatternLogEventsAssertions<TSubject, TAssertions>(
        this ReferenceTypeAssertions<TSubject, TAssertions> parent,
        IReadOnlyCollection<LogEvent> logEvents)
        where TAssertions : BaseAssertions<TSubject, TAssertions>
        => new PatternLogEventsAssertionsImpl(logEvents, parent.CurrentAssertionChain);

    public static StructuredValueAssertions CreateStructuredValueAssertions(
        this LogEventAssertionImpl logEventAssertion,
        StructureValue structureValue,
        string propertyName)
        => new StructuredValueAssertionsImpl(logEventAssertion, structureValue, propertyName);

    public static LogEventPropertyValueAssertions CreateLogEventPropertyValueAssertions(
        this LogEventAssertionImpl parent,
        string name,
        LogEventPropertyValue logEventPropertyValue)
        => new LogEventPropertyValueAssertionsImpl(parent, logEventPropertyValue, name);

    public static LogEventAssertion CreateLogEventAssertion<TSubject, TAssertions>(
        this ReferenceTypeAssertions<TSubject, TAssertions> parent,
        string messageTemplate,
        LogEvent logEvent)
        where TAssertions : BaseAssertions<TSubject, TAssertions>
        => new LogEventAssertionImpl(messageTemplate, logEvent, parent.CurrentAssertionChain);

    public static LogEventsPropertyAssertion CreateLogEventsPropertyAssertion(
        this LogEventsAssertionsImpl parent,
        string propertyName)
        => new LogEventsPropertyAssertionImpl(parent, propertyName);
}