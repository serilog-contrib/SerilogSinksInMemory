using System.Collections.Generic;
using System.Linq;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;
using Shouldly;

namespace Serilog.Sinks.InMemory.Shouldly4
{
    [ShouldlyMethods]
    public class LogEventsPropertyAssertionImpl :  LogEventsPropertyAssertion
    {
        private readonly LogEventsAssertionsImpl _logEventsAssertions;
        private readonly IEnumerable<LogEvent> _logEvents;
        private readonly string _propertyName;

        public LogEventsPropertyAssertionImpl(LogEventsAssertionsImpl logEventsAssertions, IEnumerable<LogEvent> logEvents, string propertyName)
        {
            _logEventsAssertions = logEventsAssertions;
            _logEvents = logEvents;
            _propertyName = propertyName;
        }

        public AndConstraint<LogEventsAssertions> WithValues(params object[] values)
        {
            _logEvents
                .Count()
                .ShouldBe(
                values.Length,
                $"Can't assert property values because {values.Length} values were provided while only {_logEvents.Count()} messages were expected");

            var propertyValues = _logEvents
                .Select(logEvent => GetValueFromProperty(logEvent.Properties[_propertyName]))
                .ToArray();

            var notFound = values
                .Where(v => !propertyValues.Contains(v))
                .ToArray();

            notFound.ShouldBeEmpty(
                $"Expected property values {propertyValues} to contain {values} but did not find {notFound}");

            return new AndConstraint<LogEventsAssertions>(_logEventsAssertions);
        }

        private object GetValueFromProperty(LogEventPropertyValue instance)
        {
            switch(instance)
            {
                case ScalarValue scalarValue:
                    return scalarValue.Value;
                default:
                    return _logEventsAssertions.Subject.ToString();
            }
        }
    }
}