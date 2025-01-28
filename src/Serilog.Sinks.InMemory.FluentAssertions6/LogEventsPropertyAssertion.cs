using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;

namespace Serilog.Sinks.InMemory.FluentAssertions6
{
    public class LogEventsPropertyAssertionImpl : ReferenceTypeAssertions<IEnumerable<LogEvent>, LogEventsPropertyAssertionImpl>, LogEventsPropertyAssertion
    {
        private readonly LogEventsAssertions _logEventsAssertions;
        private readonly IEnumerable<LogEvent> _logEvents;

        public LogEventsPropertyAssertionImpl(LogEventsAssertionsImpl logEventsAssertions, string propertyName)
            : base(logEventsAssertions.Subject)
        {
            _logEventsAssertions = logEventsAssertions;
            _logEvents = logEventsAssertions.Subject;
            Identifier = propertyName;
        }

        protected override string Identifier { get; }

        public AndConstraint<LogEventsAssertions> WithValues(params object[] values)
        {
            Execute.Assertion
                .ForCondition(_logEvents.Count() == values.Length)
                .FailWith(
                    $"Can't assert property values because {values.Length} values were provided while only {_logEvents.Count()} messages were expected");

            var propertyValues = _logEvents
                .Select(logEvent => GetValueFromProperty(logEvent.Properties[Identifier]))
                .ToArray();

            var notFound = values
                .Where(v => !propertyValues.Contains(v))
                .ToArray();

            Execute.Assertion
                .ForCondition(!notFound.Any())
                .FailWith("Expected property values {0} to contain {1} but did not find {2}",
                    propertyValues,
                    values,
                    notFound);

            return new AndConstraint<LogEventsAssertions>(_logEventsAssertions);
        }

        private object GetValueFromProperty(LogEventPropertyValue instance)
        {
            switch(instance)
            {
                case ScalarValue scalarValue:
                    return scalarValue.Value;
                default:
                    return Subject.ToString();
            }
        }
    }
}