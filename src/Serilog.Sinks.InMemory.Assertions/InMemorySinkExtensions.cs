using System;
using Serilog.Events;

namespace Serilog.Sinks.InMemory.Assertions
{
    public static class InMemorySinkAssertionExtensions
    {
        public static InMemorySinkAssertions Should(this InMemorySink instance)
        {
            return new InMemorySinkAssertions(instance);
        }

        public static LogEventAssertions Should(this LogEvent instance)
        {
            return new LogEventAssertions(instance);
        }
    }
}
