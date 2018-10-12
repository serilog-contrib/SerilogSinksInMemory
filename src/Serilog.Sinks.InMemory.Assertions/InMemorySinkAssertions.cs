using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class InMemorySinkAssertions  : ReferenceTypeAssertions<InMemorySink, InMemorySinkAssertions>
    {
        public InMemorySinkAssertions(InMemorySink instance)
        {
            Subject = instance;
        }

        protected override string Identifier { get; } = nameof(InMemorySink);

        public AndWhichConstraint<InMemorySinkAssertions, LogEvent> HaveMessage(
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
                    "Expected a message to be logged with template {0} but didn't find any", 
                    messageTemplate);

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(matches.Count == 1)
                .FailWith(
                    "Expected a message to be logged with template {0} exactly once, but it was found {1} times", 
                    messageTemplate,
                    matches.Count);

            return new AndWhichConstraint<InMemorySinkAssertions, LogEvent>(this, matches.Single());
        }
    }
}