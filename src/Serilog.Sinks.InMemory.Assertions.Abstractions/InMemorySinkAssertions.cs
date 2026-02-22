#nullable enable

using System;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public interface InMemorySinkAssertions : ISubjectAssertions<InMemorySink>
    {
        LogEventsAssertions HaveMessage(
            Func<LogEvent, bool> predicate,
            string? predicateErrorName = null,
            string because = "",
            params object[] becauseArgs);

        LogEventsAssertions HaveMessage(
            string messageTemplate,
            string because = "",
            params object[] becauseArgs);

        PatternLogEventsAssertions HaveMessage();

        void NotHaveMessage(
            string? messageTemplate = null,
            string because = "",
            params object[] becauseArgs);

        void NotHaveMessage(
            Func<LogEvent, bool> predicate,
            string? predicateErrorName = null,
            string because = "",
            params object[] becauseArgs);
    }
}