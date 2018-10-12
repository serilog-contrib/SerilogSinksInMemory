using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class LogEventAssertions : ReferenceTypeAssertions<LogEvent, LogEventAssertions>
    {
        public LogEventAssertions(LogEvent instance)
        {
            Subject = instance;
        }

        protected override string Identifier { get; }

        public AndWhichConstraint<LogEventAssertions, LogEventPropertyValue> HaveProperty(
            string propertyName, 
            string because = "",
            params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Properties.ContainsKey(propertyName))
                .FailWith("Expected log message to have a property {0} but it wasn't found", propertyName);

            return new AndWhichConstraint<LogEventAssertions, LogEventPropertyValue>(
                this,
                Subject.Properties[propertyName]);
        }
    }
}
