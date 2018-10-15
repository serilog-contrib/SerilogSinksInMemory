using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class LogEventsAssertions : ReferenceTypeAssertions<IEnumerable<LogEvent>, LogEventsAssertions>
    {
        private readonly string _messageTemplate;

        public LogEventsAssertions(string messageTemplate, IEnumerable<LogEvent> matches)
        {
            _messageTemplate = messageTemplate;
            Subject = matches;
        }

        protected override string Identifier { get; } = "log events";

        public LogEventsAssertions Appearing()
        {
            return this;
        }

        public LogEventAssertion Once(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Count() == 1)
                .FailWith(
                    "Expected message {0} to appear exactly once, but it was found {1} times",
                    _messageTemplate,
                    Subject.Count());

            return new LogEventAssertion(_messageTemplate, Subject.Single());
        }

        public LogEventsAssertions Times(int number, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Count() == number)
                .FailWith(
                    "Expected message {0} to appear {1} times, but it was found {2} times",
                    _messageTemplate,
                    number,
                    Subject.Count());

            return this;
        }
    }
}