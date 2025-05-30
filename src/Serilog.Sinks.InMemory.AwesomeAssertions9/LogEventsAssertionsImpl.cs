using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;

namespace Serilog.Sinks.InMemory.AwesomeAssertions9
{
    public class LogEventsAssertionsImpl : ReferenceTypeAssertions<IEnumerable<LogEvent>, LogEventsAssertionsImpl>, LogEventsAssertions
    {
        private readonly string _messageTemplate;

        public LogEventsAssertionsImpl(string messageTemplate, IEnumerable<LogEvent> matches, AssertionChain assertionChain) : base(matches, assertionChain)
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
            CurrentAssertionChain
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Count() == 1)
                .FailWith(
                    "Expected message {0} to appear exactly once, but it was found {1} times",
                    _messageTemplate,
                    Subject.Count());

            return new LogEventAssertionImpl(_messageTemplate, Subject.Single(), CurrentAssertionChain);
        }

        public LogEventsAssertions Times(int number, string because = "", params object[] becauseArgs)
        {
            CurrentAssertionChain
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

            CurrentAssertionChain
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.All(logEvent => logEvent.Level == level))
                .FailWith($"Expected instances of log message {{0}} to have level {{1}}, but found {notMatchedText}",
                    _messageTemplate,
                    level.ToString());

            return this;
        }

        public LogEventsPropertyAssertion WithProperty(string propertyName, string because = "", params object[] becauseArgs)
        {
            CurrentAssertionChain
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.All(logEvent => logEvent.Properties.ContainsKey(propertyName)))
                .FailWith($"Expected all instances of log message {{0}} to have property {{1}}, but it was not found",
                    _messageTemplate,
                    propertyName);

            return new LogEventsPropertyAssertionImpl(this, propertyName, CurrentAssertionChain);
        }
    }
}