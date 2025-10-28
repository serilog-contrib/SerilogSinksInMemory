using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;

namespace Serilog.Sinks.InMemory
{
    public static class InMemorySinkExtensions
    {
        /// <summary>
        /// Writes log events to an in-memory log sink.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration InMemory(
            this LoggerSinkConfiguration sinkConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));

            return sinkConfiguration.Sink(InMemorySink.Instance, restrictedToMinimumLevel, levelSwitch);
        }
    }
}
