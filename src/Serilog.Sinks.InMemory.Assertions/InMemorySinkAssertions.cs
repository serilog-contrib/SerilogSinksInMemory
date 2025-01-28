using System.Linq;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Serilog.Sinks.InMemory.Assertions
{
    public interface InMemorySinkAssertions
    {
        LogEventsAssertions HaveMessage(
            string messageTemplate,
            string because = "",
            params object[] becauseArgs);

        PatternLogEventsAssertions HaveMessage();

        void NotHaveMessage(
            string messageTemplate = null,
            string because = "",
            params object[] becauseArgs);
    }
}