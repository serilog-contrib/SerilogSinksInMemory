using System;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;
using Shouldly;

namespace Serilog.Sinks.InMemory.Shouldly4
{
    public class LogEventPropertyValueAssertionsImpl : LogEventPropertyValueAssertions
    {
        private readonly LogEventAssertionImpl _logEventAssertion;
        private readonly string _propertyName;
        private readonly LogEventPropertyValue _subject;

        public LogEventPropertyValueAssertionsImpl(LogEventAssertionImpl logEventAssertion,
            LogEventPropertyValue instance, string propertyName)
        {
            _logEventAssertion = logEventAssertion;
            _propertyName = propertyName;
            _subject = instance;
        }

        public TValue WhichValue<TValue>()
        {
            if (_subject is ScalarValue scalarValue)
            {
                if (scalarValue.Value is not TValue value)
                {
                    throw new ShouldAssertException(
                        $"Expected property value to be of type \"{typeof(TValue).Name}\" but found \"{scalarValue.Value?.GetType().Name ?? "null"}\"");
                }

                return value;
            }

            throw new Exception(
                $"Expected property value to be of type {typeof(TValue).Name} but the property value is not a scalar and I don't know how to handle that");
        }

        public LogEventAssertion WithValue(object value, string because = "",
            params object[] becauseArgs)
        {
            var actualValue = GetValueFromProperty(_subject);

            if (!Equals(actualValue, value))
            {
                var formattedValue = value is string ? $"\"{value}\"" : value.ToString();
                var formattedActualValue = actualValue is string ? $"\"{actualValue}\"" : actualValue.ToString();
                
                throw new ShouldAssertException(
                    $"Expected property \"{_propertyName}\" to have value {formattedValue} but found {formattedActualValue}");
            }

            return _logEventAssertion;
        }

        private object GetValueFromProperty(LogEventPropertyValue instance)
        {
            switch (instance)
            {
                case ScalarValue scalarValue:
                    return scalarValue.Value;
                default:
                    return _subject.ToString();
            }
        }

        public StructuredValueAssertions HavingADestructuredObject(string because = "", params object[] becauseArgs)
        {
            if (_subject is not StructureValue structureValue)
            {
                throw new ShouldAssertException(
                    $"Expected message \"{_logEventAssertion.Subject.MessageTemplate}\" to have a property \"{_propertyName}\" that holds a destructured object but found a scalar value");
            }

            return new StructuredValueAssertionsImpl(structureValue, _propertyName, _logEventAssertion);
        }
    }
}