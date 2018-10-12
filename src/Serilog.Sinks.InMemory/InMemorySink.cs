using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.InMemory
{
    public class InMemorySink : ILogEventSink
    {
        private readonly List<LogEvent> _logEvents;

        public InMemorySink()
        {
            _logEvents = new List<LogEvent>();
        }

        public IEnumerable<LogEvent> LogEvents => _logEvents.AsReadOnly();

        public void Emit(LogEvent logEvent)
        {
            _logEvents.Add(logEvent);
        }
    }
}