using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions.Execution;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;

namespace Serilog.Sinks.InMemory.AwesomeAssertions9
{
    public class PatternLogEventsAssertionsImpl : LogEventsAssertionsImpl, PatternLogEventsAssertions
    {
        public PatternLogEventsAssertionsImpl(IEnumerable<LogEvent> subjectLogEvents, AssertionChain assertionChain) : base(null, subjectLogEvents, assertionChain)
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

            CurrentAssertionChain
                .BecauseOf(because, becauseArgs)
                .ForCondition(matches.Any())
                .FailWith(
                    "Expected a message with pattern {0} to be logged",
                    pattern);

            return new LogEventsAssertionsImpl(pattern, matches, CurrentAssertionChain);
        }
    }
}