using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;

namespace Serilog.Sinks.InMemory.FluentAssertions8
{
    public class LogEventAssertionImpl : ReferenceTypeAssertions<LogEvent, LogEventAssertionImpl>, LogEventAssertion
    {
        private readonly string _messageTemplate;

        public LogEventAssertionImpl(string messageTemplate, LogEvent subject, AssertionChain assertionChain) : base(subject, assertionChain)
        {
            _messageTemplate = messageTemplate;
        }

        protected override string Identifier => "log event";

        public LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs)
        {
            if (name.StartsWith("@"))
            {
                name = name.Substring(1);
            }
            
            CurrentAssertionChain
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Properties.ContainsKey(name))
                .FailWith("Expected message {0} to have a property {1} but it wasn't found",
                    _messageTemplate,
                    name);

            return new LogEventPropertyValueAssertionsImpl(
                this,
                Subject.Properties[name],
                name);
        }

        public LogEventAssertion WithLevel(LogEventLevel level,  string because = "", params object[] becauseArgs)
        {
            CurrentAssertionChain
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Level == level)
                .FailWith("Expected message {0} to have level {1}, but it is {2}",
                    _messageTemplate,
                    level.ToString(),
                    Subject.Level.ToString());

            return this;
        }
    }
}