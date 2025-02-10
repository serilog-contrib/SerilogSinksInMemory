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
                scalarValue.ShouldBeOfType<TValue>(
                    $"Expected property value to be of type {typeof(TValue).Name} but found {scalarValue.Value.GetType().Name}");

                return (TValue)scalarValue.Value;
            }

            throw new Exception(
                $"Expected property value to be of type {typeof(TValue).Name} but the property value is not a scalar and I don't know how to handle that");
        }

        public AndConstraint<LogEventAssertion> WithValue(object value, string because = "",
            params object[] becauseArgs)
        {
            var actualValue = GetValueFromProperty(_subject);

            actualValue
                .ShouldBe(
                    value,
                    $"Expected property {_propertyName} to have value '{value}' but found '{actualValue}'");

            return new AndConstraint<LogEventAssertion>(_logEventAssertion);
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
            _subject
                .ShouldBeOfType<StructureValue>(
                    $"Expected message '{_logEventAssertion.Subject.MessageTemplate}' to have a property '{_propertyName}' that holds a destructured object but found a scalar value");

            return new StructuredValueAssertionsImpl(_subject as StructureValue, _propertyName, _logEventAssertion);
        }
    }
}