using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public class InMemorySinkAssertions : ReferenceTypeAssertions<InMemorySink, InMemorySinkAssertions>
    {
        public InMemorySinkAssertions(InMemorySink instance)
        {
            Subject = SnapshotOf(instance);
        }
        
        /*
         * Hack attack.
         *
         * This is a bit of a dirty way to work around snapshotting the InMemorySink instance
         * to ensure that you won't get hit by an InvalidOperationException when calling
         * HaveMessage() and the logger gets called from somewhere else and adds a new
         * LogEvent to the collection while that method is invoked.
         *
         * For now we copy the LogEvents from the current sink and use reflection to assign
         * it to a new instance of InMemorySink that will be used by the assertions,
         * effectively snapshotting the InMemorySink that was used by the tests.
         */
        private static InMemorySink SnapshotOf(InMemorySink instance)
        {
            // Capture the current log events
            var logEvents = instance.LogEvents.ToList();

            // Create a new sink instance
            var snapshot = new InMemorySink();

            // Get the field that holds the collection of log events
            var field = snapshot.GetType().GetField("_logEvents", BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                throw new InvalidOperationException(
                    "Can't snapshot the InMemorySink instance because the private field that holds the messages could not be found.");
            }

            // Assign the snapshot of log events to the field
            ((List<LogEvent>)field.GetValue(snapshot)).AddRange(logEvents);

            // Return the new snapshotted InMemorySink instance
            return snapshot;
        }

        protected override string Identifier => nameof(InMemorySink);

        public LogEventsAssertions HaveMessage(
            string messageTemplate,
            string because = "",
            params object[] becauseArgs)
        {
            var matches = Subject
                .LogEvents
                .Where(logEvent => logEvent.MessageTemplate.Text == messageTemplate)
                .ToList();

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(matches.Any())
                .FailWith(
                    "Expected message {0} to be logged",
                    messageTemplate);

            return new LogEventsAssertions(messageTemplate, matches);
        }

        public PatternLogEventsAssertions HaveMessage()
        {
            return new PatternLogEventsAssertions(Subject.LogEvents);
        }

        public void NotHaveMessage(
            string messageTemplate = null,
            string because = "",
            params object[] becauseArgs)
        {
            int count;
            string failureMessage;

            if (messageTemplate != null)
            {
                count = Subject
                .LogEvents
                .Count(logEvent => logEvent.MessageTemplate.Text == messageTemplate);
                
                failureMessage = $"Expected message \"{messageTemplate}\" not to be logged, but it was found {(count > 1 ? $"{count} times" : "once")}";
            }
            else
            {
                count = Subject
                    .LogEvents
                    .Count();

                failureMessage = $"Expected no messages to be logged, but found {(count > 1 ? $"{count} messages" : "message")}";
            }

            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .ForCondition(count == 0)
                .FailWith(failureMessage);
        }
    }
}