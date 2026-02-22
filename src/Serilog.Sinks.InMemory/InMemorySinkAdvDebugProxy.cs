#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

namespace Serilog.Sinks.InMemory
{
    internal sealed class InMemorySinkAdvDebugProxy(InMemorySink sink)
        : List<DebugLogEvent>(sink.LogEvents.Select(logEvent => new DebugLogEvent(logEvent)));

    internal sealed class DebugLogEvent(LogEvent logEvent)
    {
        public LogEventLevel Level => logEvent.Level;
        public string Message => logEvent.RenderMessage();

        public string MessageTemplate => logEvent.MessageTemplate.ToString();

        public IReadOnlyDictionary<string, LogEventPropertyValue> Properties => logEvent.Properties;

        public Exception? Exception => logEvent.Exception;

        public LogEvent OriginalLogEvent => logEvent;

        public override string ToString() => Message;
    }
}