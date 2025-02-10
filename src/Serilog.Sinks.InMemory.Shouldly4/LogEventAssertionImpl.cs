using System;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;
using Shouldly;

namespace Serilog.Sinks.InMemory.Shouldly4
{
    [ShouldlyMethods]
    public class LogEventAssertionImpl : LogEventAssertion
    {
        private readonly string _messageTemplate;
        private readonly LogEvent _subject;

        public LogEventAssertionImpl(string messageTemplate, LogEvent subject)
        {
            _messageTemplate = messageTemplate;
            _subject = subject;
        }

        public LogEvent Subject => _subject;

        public LogEventPropertyValueAssertions WithProperty(string name, string because = "", params object[] becauseArgs)
        {
            if (name.StartsWith("@"))
            {
                name = name.Substring(1);
            }
            
            Subject
                .Properties
                .ShouldContain(kv => kv.Key == name,
                    $"Expected message {_messageTemplate} to have a property {name} but it wasn't found");

            return new LogEventPropertyValueAssertionsImpl(
                this,
                Subject.Properties[name],
                name);
        }

        public LogEventAssertion WithLevel(LogEventLevel level,  string because = "", params object[] becauseArgs)
        {
            Subject
                .Level
                .ShouldBe(
                    level,
                    $"Expected message {_messageTemplate} to have level {level.ToString()}, but it is {Subject.Level.ToString()}");

            return this;
        }

        public void Match(Func<LogEvent, bool> predicate)
        {
            Subject.ShouldSatisfyAllConditions(logEvent => predicate(logEvent).ShouldBeTrue());
        }
    }
}