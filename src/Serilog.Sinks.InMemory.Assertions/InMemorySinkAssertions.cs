using System.Linq;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class InMemorySinkAssertions  : ReferenceTypeAssertions<InMemorySink, InMemorySinkAssertions>
    {
        public InMemorySinkAssertions(InMemorySink instance)
        {
            Subject = instance;
        }

        protected override string Identifier { get; } = nameof(InMemorySink);

        public LogEventsAssertions HaveMessage(
            string messageTemplate,
            string because = "",
            params object[] becauseArgs)
        {
            var matches = Subject
                .LogEvents
                .Where(logEvent => logEvent.MessageTemplate.Text == messageTemplate)
                .ToList();

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(matches.Any())
                .FailWith(
                    "Expected message {0} to be logged",
                    messageTemplate);

            return new LogEventsAssertions(messageTemplate, matches);
        }
    }
}