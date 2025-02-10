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

        public LogEventsAssertions WithValues(params object[] values)
        {
            if (_logEvents.Count() != values.Length)
            {
                throw new ShouldAssertException(
                    $"Can't assert property values because {values.Length} values were provided while only {_logEvents.Count()} messages were expected");
            }

            var propertyValues = _logEvents
                .Select(logEvent => GetValueFromProperty(logEvent.Properties[_propertyName]))
                .ToArray();

            var notFound = values
                .Where(v => !propertyValues.Contains(v))
                .ToArray();

            if (notFound.Any())
            {
                var formattedPropertyValues = "{" + string.Join(", ", propertyValues.Select(p => $"\"{p}\"")) + "}";
                var formattedValues =  "{" + string.Join(", ", values.Select(p => $"\"{p}\"")) + "}";
                var formattedNotFound =  "{" + string.Join(", ", notFound.Select(p => $"\"{p}\"")) + "}";
                throw new ShouldAssertException(
                    $"Expected property values {formattedPropertyValues} to contain {formattedValues} but did not find {formattedNotFound}");
            }

            return _logEventsAssertions;
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