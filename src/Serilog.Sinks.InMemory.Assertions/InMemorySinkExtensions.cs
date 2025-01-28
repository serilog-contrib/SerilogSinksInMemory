using System;

namespace Serilog.Sinks.InMemory.Assertions
{
    public static class InMemorySinkAssertionExtensions
    {
        public static InMemorySinkAssertions Should(this InMemorySink instance)
        {
            throw new NotImplementedException();
            //return new InMemorySinkAssertions(SnapshotOf(instance));
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
    }
}
