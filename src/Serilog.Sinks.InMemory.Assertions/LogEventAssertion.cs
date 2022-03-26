using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class LogEventAssertion : ReferenceTypeAssertions<LogEvent, LogEventAssertion>
    {
        private readonly string _messageTemplate;

        public LogEventAssertion(string messageTemplate, LogEvent subject) : base(subject)
        {
            _messageTemplate = messageTemplate;
        }

        protected override string Identifier => "log event";

        public LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Properties.ContainsKey(name))
                .FailWith("Expected message {0} to have a property {1} but it wasn't found",
                    _messageTemplate,
                    name);

            return new LogEventPropertyValueAssertions(
                this,
                Subject.Properties[name],
                name);
        }

        public LogEventAssertion WithLevel(LogEventLevel level,  string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Level == level)
                .FailWith("Expected message {0} to have level {1}, but it is {2}",
                    _messageTemplate,
                    level.ToString(),
                    Subject.Level.ToString());

            return this;
        }

        public LogEventExceptionAssertion WithException<TException>(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Exception != null)
                .FailWith("Expected message {0} to have an exception but it doesn't",
                    _messageTemplate);

            var exceptionType = typeof(TException);
            var logEventExceptionType = Subject.Exception.GetType();

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(logEventExceptionType == exceptionType)
                .FailWith("Expected message {0} to have an exception of type {1} but found {2}",
                    _messageTemplate,
                    exceptionType.Name,
                    logEventExceptionType.Name);

            return new LogEventExceptionAssertion(Subject.Exception, exceptionType, Subject);
        }
    }
}