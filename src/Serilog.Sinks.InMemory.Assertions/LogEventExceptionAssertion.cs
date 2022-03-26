using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using FluentAssertions.Formatting;
using FluentAssertions.Primitives;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class LogEventExceptionAssertion : ReferenceTypeAssertions<Exception, LogEventExceptionAssertion>
    {
        public LogEventExceptionAssertion(Exception subject, Type exceptionType, LogEvent logEvent) : base(subject)
        {
            ExceptionType = exceptionType;
            LogEvent = logEvent;
        }

        public Type ExceptionType { get; }
        public LogEvent LogEvent { get; }

        protected override string Identifier => "log event exception";

        public LogEventExceptionAssertion AndMessage(string message, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.Message == message)
                .FailWith("Expected exception to have message {0} but found {1}",
                    message,
                    Subject.Message);

            return this;
        }

        public AndWhichConstraint<LogEventExceptionAssertion, Exception> AndInnerException<TException>(string because = "",
            params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(Subject.InnerException != null)
                .FailWith("Expected exception to have an inner exception but it doesn't");

            var exceptionType = typeof(TException);
            var logEventExceptionType = Subject.InnerException.GetType();

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(logEventExceptionType == exceptionType)
                .FailWith("Expected exception to have an inner exception of type {0} but found {1}",
                    exceptionType.Name,
                    logEventExceptionType.Name);

            return new AndWhichConstraint<LogEventExceptionAssertion, Exception>(this, Subject.InnerException);
        }

        public LogEventExceptionAssertion ContainingException(Func<Exception, bool> predicate, string because = "", params object[] becauseArgs)
        {
            if (Subject is AggregateException aggregateException)
            {
                var match = aggregateException.InnerExceptions.Where(predicate).SingleOrDefault();
                
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .ForCondition(match != null)
                    .FailWith("Expected aggregate exception to have an exception matching {0}",
                        predicate.ToString());
                
                return new LogEventExceptionAssertion(match, match.GetType(), LogEvent);
            }

            throw new AssertionFailedException("Subject is not an AggregateException");
        }
    }
}