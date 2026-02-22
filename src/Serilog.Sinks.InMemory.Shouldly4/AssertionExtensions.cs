// ReSharper disable CoVariantArrayConversion

using System.Collections;

namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

internal static class AssertionExtensions
{
    public static readonly AssertionFramework AssertionFramework =
        new(AssertionFrameworks.Shouldly, new Version(4, 0));

    public static void Assert(this FailMessage failureMessage, [DoesNotReturnIf(false)] bool condition, string because = "", params object[] becauseArgs)
    {
        if (condition)
        {
            return;
        }

        var message = failureMessage.Message;
        if (failureMessage.Args.Length != 0)
        {
            message = string.Format(message, FormatArgs(failureMessage.Args));
        }

        throw new ShouldAssertException(message);
    }

    private static string[] FormatArgs(IEnumerable args)
        => args.Cast<object>().Select(arg => arg switch
        {
            null => null,
            string str => $"\"{str}\"",
            _ => typeof(IEnumerable).IsAssignableFrom(arg.GetType()) ? $"{{{string.Join(", ", FormatArgs((IEnumerable)arg))}}}" : arg.ToString(),
        }).ToArray();

    public static LogEventsAssertions CreateLogEventsAssertions<TSubject>(
        this BaseShouldlyAssertions<TSubject> parent,
        string pattern,
        IReadOnlyCollection<LogEvent> logEvents)
        => new LogEventsAssertionsImpl(pattern, logEvents);

    public static PatternLogEventsAssertions CreatePatternLogEventsAssertions<TSubject>(
        this BaseShouldlyAssertions<TSubject> parent,
        IReadOnlyCollection<LogEvent> logEvents)
        => new PatternLogEventsAssertionsImpl(logEvents);

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

    public static LogEventAssertion CreateLogEventAssertion<TSubject>(
        this BaseShouldlyAssertions<TSubject> parent,
        string messageTemplate,
        LogEvent logEvent)
        => new LogEventAssertionImpl(messageTemplate, logEvent);

    public static LogEventsPropertyAssertion CreateLogEventsPropertyAssertion(
        this LogEventsAssertionsImpl parent,
        string propertyName)
        => new LogEventsPropertyAssertionImpl(parent, propertyName);
}