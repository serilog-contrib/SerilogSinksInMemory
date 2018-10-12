using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class MessageAssertions : ReferenceTypeAssertions<IEnumerable<LogEvent>, MessageAssertions>
    {
        private readonly string _messageTemplate;

        public MessageAssertions(List<LogEvent> matches, string messageTemplate)
        {
            Subject = matches;
            _messageTemplate = messageTemplate;
        }

        protected override string Identifier { get; } = "logEvent";

        public AndWhichConstraint<MessageAssertions, LogEvent> AppearsOnce(string because = "",
            params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Count() == 1)
                .FailWith(
                    "Expected a message to be logged with template {0} exactly once, but it was found {1} times",
                    _messageTemplate,
                    Subject.Count());

            return new AndWhichConstraint<MessageAssertions, LogEvent>(this, Subject.Single());
        }

        public AndWhichConstraint<MessageAssertions, IEnumerable<LogEvent>> AppearsTimes(
            int times,
            string because = "",
            params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Count() == times)
                .FailWith(
                    "Expected a message to be logged with template {0} exactly {1} times, but it was found {2} times",
                    _messageTemplate,
                    times,
                    Subject.Count());

            return new AndWhichConstraint<MessageAssertions, IEnumerable<LogEvent>>(this, Subject);
        }
    }
}