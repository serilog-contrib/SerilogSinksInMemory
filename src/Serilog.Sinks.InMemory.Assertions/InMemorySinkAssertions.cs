using System.Linq;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class InMemorySinkAssertions : ReferenceTypeAssertions<InMemorySink, InMemorySinkAssertions>
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

        public PatternLogEventsAssertions HaveMessage()
        {
            return new PatternLogEventsAssertions(Subject.LogEvents);
        }

        public void NotHaveMessage(
            string messageTemplate = null,
            string because = "",
            params object[] becauseArgs)
        {
            int count;
            string failureMessage;

            if (messageTemplate != null)
            {
                count = Subject
                .LogEvents
                .Count(logEvent => logEvent.MessageTemplate.Text == messageTemplate);
                
                failureMessage = $"Expected message \"{messageTemplate}\" not to be logged, but it was found {(count > 1 ? $"{count} times" : "once")}";
            }
            else
            {
                count = Subject
                    .LogEvents
                    .Count();

                failureMessage = $"Expected no messages to be logged, but found {(count > 1 ? $"{count} messages" : "message")}";
            }

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(count == 0)
                .FailWith(failureMessage);
        }
    }
}