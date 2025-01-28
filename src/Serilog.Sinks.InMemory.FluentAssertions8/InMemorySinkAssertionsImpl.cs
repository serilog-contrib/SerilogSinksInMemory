﻿using System.Linq;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Serilog.Sinks.InMemory.Assertions;
using Serilog.Sinks.InMemory.Assertions.Abstractions;

namespace Serilog.Sinks.InMemory.FluentAssertions8
{
    public class InMemorySinkAssertionsImpl : ReferenceTypeAssertions<InMemorySink, InMemorySinkAssertionsImpl>, InMemorySinkAssertions
    {
        public InMemorySinkAssertionsImpl(InMemorySink snapshotInstance) : base(snapshotInstance, AssertionChain.GetOrCreate())
        {
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
         * effectively creating a snapshot of the InMemorySink that was used by the tests.
         */
        private static InMemorySink SnapshotOf(InMemorySink instance)
        {
            return instance.Snapshot();
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

            CurrentAssertionChain
                .BecauseOf(because, becauseArgs)
                .ForCondition(matches.Any())
                .FailWith(
                    "Expected message {0} to be logged",
                    messageTemplate);

            return new LogEventsAssertionsImpl(messageTemplate, matches, CurrentAssertionChain);
        }

        public PatternLogEventsAssertions HaveMessage()
        {
            return new PatternLogEventsAssertionsImpl(Subject.LogEvents, CurrentAssertionChain);
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

            CurrentAssertionChain
                .BecauseOf(because, becauseArgs)
                .ForCondition(count == 0)
                .FailWith(failureMessage);
        }
    }
}