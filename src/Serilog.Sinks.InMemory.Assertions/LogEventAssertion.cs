using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class LogEventAssertion : ReferenceTypeAssertions<LogEvent, LogEventAssertion>
    {
        private readonly string _messageTemplate;

        public LogEventAssertion(string messageTemplate, LogEvent subject)
        {
            _messageTemplate = messageTemplate;
            Subject = subject;
        }

        protected override string Identifier { get; } = "log event";

        public LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Properties.ContainsKey(name))
                .FailWith("Expected message {0} to have a property {1} but it wasn't found",
                    _messageTemplate,
                    name);

            return new LogEventPropertyValueAssertions(
                Subject.Properties[name],
                name);
        }
    }
}