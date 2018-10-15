using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class LogEventPropertyValueAssertions : ReferenceTypeAssertions<LogEventPropertyValue, LogEventPropertyValueAssertions>
    {
        public LogEventPropertyValueAssertions(LogEventPropertyValue instance, string propertyName)
        {
            Subject = instance;
            Identifier = propertyName;
        }

        protected override string Identifier { get; }

        public void WithValue(object value, string because = "", params object[] becauseArgs)
        {
            var actualValue = GetValueFromProperty(Subject);

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Equals(actualValue, value))
                .FailWith("Expected property {0} to have value {1} but found {2}",
                    Identifier,
                    value,
                    actualValue);
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