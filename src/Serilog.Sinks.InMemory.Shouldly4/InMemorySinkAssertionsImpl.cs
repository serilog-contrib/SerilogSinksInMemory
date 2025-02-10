using System.Linq;
using Serilog.Sinks.InMemory.Assertions;
using Shouldly;

namespace Serilog.Sinks.InMemory.Shouldly4
{
    [ShouldlyMethods] 
    public class InMemorySinkAssertionsImpl : InMemorySinkAssertions
    {
        private readonly InMemorySink _snapshotInstance;

        public InMemorySinkAssertionsImpl(InMemorySink snapshotInstance)
        {
            _snapshotInstance = snapshotInstance;
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

        public LogEventsAssertions HaveMessage(
            string messageTemplate,
            string because = "",
            params object[] becauseArgs)
        {
            var matches = _snapshotInstance
                .LogEvents
                .Where(logEvent => logEvent.MessageTemplate.Text == messageTemplate)
                .ToList();

            matches.ShouldNotBeEmpty($"Expected message {messageTemplate} to be logged");

            return new LogEventsAssertionsImpl(messageTemplate, matches);
        }

        public PatternLogEventsAssertions HaveMessage()
        {
            return new PatternLogEventsAssertionsImpl(_snapshotInstance.LogEvents);
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
                count = _snapshotInstance
                .LogEvents
                .Count(logEvent => logEvent.MessageTemplate.Text == messageTemplate);
                
                failureMessage = $"Expected message \"{messageTemplate}\" not to be logged, but it was found {(count > 1 ? $"{count} times" : "once")}";
            }
            else
            {
                count = _snapshotInstance
                    .LogEvents
                    .Count();

                failureMessage = $"Expected no messages to be logged, but found {(count > 1 ? $"{count} messages" : "message")}";
            }

            count.ShouldBe(0, failureMessage);
        }
    }
}