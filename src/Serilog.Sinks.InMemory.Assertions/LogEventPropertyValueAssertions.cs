using System.IO;
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

        public void WithValue(string value, string because = "", params object[] becauseArgs)
        {
            var textWriter = new StringWriter();
            Subject.Render(textWriter);
            var actualValue = textWriter.GetStringBuilder().ToString().Replace("\"", "");

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(actualValue == value)
                .FailWith("Expected property {0} to have value {1} but found {2}",
                    Identifier,
                    value,
                    actualValue);
        }
    }
}