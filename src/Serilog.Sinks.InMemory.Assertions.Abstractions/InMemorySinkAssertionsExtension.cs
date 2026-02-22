#nullable enable
using System;

namespace Serilog.Sinks.InMemory.Assertions;

/// <summary>
/// InMemory logger assertion extensibility extension.
/// </summary>
public static class InMemorySinkAssertionsExtensions
{
    public static AssertionExtension<InMemorySinkAssertions> ToAssertion(
        this InMemorySinkAssertions assertions)
    {
        if (assertions == null) throw new ArgumentNullException(nameof(assertions));
        return new AssertionExtension<InMemorySinkAssertions>(assertions, (IInMemorySinkAssertionsExtension)assertions);
    }

    public static AssertionExtension<PatternLogEventsAssertions> ToAssertion(
        this PatternLogEventsAssertions assertions)
    {
        if (assertions == null) throw new ArgumentNullException(nameof(assertions));
        return new AssertionExtension<PatternLogEventsAssertions>(assertions, (IInMemorySinkAssertionsExtension)assertions);
    }

    public static AssertionExtension<LogEventsAssertions> ToAssertion(
        this LogEventsAssertions assertions)
    {
        if (assertions == null) throw new ArgumentNullException(nameof(assertions));
        return new AssertionExtension<LogEventsAssertions>(assertions, (IInMemorySinkAssertionsExtension)assertions);
    }
}