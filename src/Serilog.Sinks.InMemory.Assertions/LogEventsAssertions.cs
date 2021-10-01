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

        public LogEventsAssertions(string messageTemplate, IEnumerable<LogEvent> matches) : base(matches)
        {
            _messageTemplate = messageTemplate;
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

        public LogEventsAssertions WithLevel(LogEventLevel level, string because = "", params object[] becauseArgs)
        {
            var notMatched = Subject.Where(logEvent => logEvent.Level != level).ToList();

            var notMatchedText = "";

            if(notMatched.Any())
            {
                notMatchedText = string.Join(" and ",
                notMatched
                .GroupBy(logEvent => logEvent.Level,
                logEvent => logEvent,
                (key, values) => $"{values.Count()} with level \"{key}\""));
            }

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.All(logEvent => logEvent.Level == level))
                .FailWith($"Expected instances of log message {{0}} to have level {{1}}, but found {notMatchedText}",
                    _messageTemplate,
                    level.ToString());

            return this;
        }

        public LogEventsPropertyAssertion WithProperty(string propertyName, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.All(logEvent => logEvent.Properties.ContainsKey(propertyName)))
                .FailWith($"Expected all instances of log message {{0}} to have property {{1}}, but it was not found",
                    _messageTemplate,
                    propertyName);

            return new LogEventsPropertyAssertion(this, propertyName);
        }
    }
}