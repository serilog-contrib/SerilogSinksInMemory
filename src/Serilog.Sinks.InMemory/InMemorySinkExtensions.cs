using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;

namespace Serilog.Sinks.InMemory
{
    public static class InMemorySinkExtensions
    {
        const string DefaultOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        /// <summary>
        /// Writes log events to an in-memory log sink.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is <code>"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"</code>.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <returns></returns>
        public static LoggerConfiguration InMemory(
            this LoggerSinkConfiguration sinkConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            LoggingLevelSwitch levelSwitch = null)
        {

            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (outputTemplate == null) throw new ArgumentNullException(nameof(outputTemplate));

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);

            return sinkConfiguration.InMemory(formatter, restrictedToMinimumLevel, levelSwitch);
        }

        /// <summary>
        /// Writes log events to an in-memory log sink.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="formatter">Controls the rendering of log events into text, for example to log JSON. To
        /// control plain text formatting, use the overload that accepts an output template.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for
        /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the pass-through minimum level
        /// to be changed at runtime.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration InMemory(
            this LoggerSinkConfiguration sinkConfiguration,
            ITextFormatter formatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            return sinkConfiguration.Sink(InMemorySink.GetInstance(formatter), restrictedToMinimumLevel, levelSwitch);
        }
    }
}
