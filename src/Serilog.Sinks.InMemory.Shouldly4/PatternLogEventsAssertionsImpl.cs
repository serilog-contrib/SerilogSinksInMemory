using System.Collections.Generic;
using System.Linq;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;
using Shouldly;

namespace Serilog.Sinks.InMemory.Shouldly4
{
    public class PatternLogEventsAssertionsImpl : LogEventsAssertionsImpl, PatternLogEventsAssertions
    {
        public PatternLogEventsAssertionsImpl(IEnumerable<LogEvent> subjectLogEvents) : base(null, subjectLogEvents)
        {
        }

        public LogEventsAssertions Containing(
            string pattern,
            string because = "",
            params object[] becauseArgs)
        {
            var matches = Subject
                .Where(logEvent => logEvent.MessageTemplate.Text.Contains(pattern))
                .ToList();

            if (matches.Count == 0)
            {
                throw new ShouldAssertException($"Expected a message with pattern \"{pattern}\" to be logged");
            }

            return new LogEventsAssertionsImpl(pattern, matches);
        }
    }
}