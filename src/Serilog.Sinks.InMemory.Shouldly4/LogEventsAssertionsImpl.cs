﻿using System.Collections.Generic;
using System.Linq;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;
using Shouldly;

namespace Serilog.Sinks.InMemory.Shouldly4
{
    [ShouldlyMethods]
    public class LogEventsAssertionsImpl : LogEventsAssertions
    {
        private readonly string _messageTemplate;
        private readonly IEnumerable<LogEvent> _matches;

        public LogEventsAssertionsImpl(string messageTemplate, IEnumerable<LogEvent> matches)
        {
            _messageTemplate = messageTemplate;
            _matches = matches;
        }

        public IEnumerable<LogEvent> Subject => _matches;

        public LogEventsAssertions Appearing()
        {
            return this;
        }

        public LogEventAssertion Once(string because = "", params object[] becauseArgs)
        {
            var actualNumberOfMessages = Subject.Count();

            if (actualNumberOfMessages != 1)
            {
                throw new ShouldAssertException(
                    $"Expected message \"{_messageTemplate}\" to appear exactly once, but it was found {actualNumberOfMessages} times");
            }

            return new LogEventAssertionImpl(_messageTemplate, Subject.Single());
        }

        public LogEventsAssertions Times(int number, string because = "", params object[] becauseArgs)
        {
            var actualNumberOfMessages = Subject.Count();
            
            if(actualNumberOfMessages != number)
            {
                throw new ShouldAssertException($"Expected message \"{_messageTemplate}\" to appear {number} times, but it was found {actualNumberOfMessages} times");
            }
            
            return this;
        }

        public LogEventsAssertions WithLevel(LogEventLevel level, string because = "", params object[] becauseArgs)
        {
            var notMatched = Subject.Where(logEvent => logEvent.Level != level).ToList();

            var notMatchedText = "";

            if(notMatched.Any())
            {
                notMatchedText = string.Join(" and ",
                notMatched
                .GroupBy(logEvent => logEvent.Level,
                logEvent => logEvent,
                (key, values) => $"{values.Count()} with level \"{key}\""));
            }

            if (Subject.Any(logEvent => logEvent.Level != level))
            {
                throw new ShouldAssertException(
                    $"Expected instances of log message \"{_messageTemplate}\" to have level \"{level.ToString()}\", but found {notMatchedText}");
            }

            return this;
        }

        public LogEventsPropertyAssertion WithProperty(string propertyName, string because = "", params object[] becauseArgs)
        {
            if(!Subject.All(logEvent => logEvent.Properties.ContainsKey(propertyName)))
            {
                throw new ShouldAssertException(
                    $"Expected all instances of log message \"{_messageTemplate}\" to have property \"{propertyName}\", but it was not found");
            }

            return new LogEventsPropertyAssertionImpl(this, Subject, propertyName);
        }
    }
}