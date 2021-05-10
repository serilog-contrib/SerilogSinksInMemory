using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class LogEventPropertyValueAssertions : ReferenceTypeAssertions<LogEventPropertyValue, LogEventPropertyValueAssertions>
    {
        private readonly LogEventAssertion _logEventAssertion;

        public LogEventPropertyValueAssertions(LogEventAssertion logEventAssertion, LogEventPropertyValue instance, string propertyName)
        {
            _logEventAssertion = logEventAssertion;
            Subject = instance;
            Identifier = propertyName;
        }

        protected override string Identifier { get; }

        public AndConstraint<LogEventAssertion> WithValue(object value, string because = "", params object[] becauseArgs)
        {
            var actualValue = GetValueFromProperty(Subject);

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Equals(actualValue, value))
                .FailWith("Expected property {0} to have value {1} but found {2}",
                    Identifier,
                    value,
                    actualValue);

            return new AndConstraint<LogEventAssertion>(_logEventAssertion);
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