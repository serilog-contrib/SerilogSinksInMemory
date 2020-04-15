using System;
using System.Collections.Generic;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.InMemory
{
    public class InMemorySink : ILogEventSink, IDisposable
    {
#if NET45
        // AsyncLocal<T> is introduced in net462 so we need to
        // be satisfied with ThreadLocal<T> for net45*
        private static readonly ThreadLocal<InMemorySink> LocalInstance = new ThreadLocal<InMemorySink>();
#else
        private static readonly AsyncLocal<InMemorySink> LocalInstance = new AsyncLocal<InMemorySink>();
#endif
        private readonly List<LogEvent> _logEvents;

        public InMemorySink()
        {
            _logEvents = new List<LogEvent>();
        }

        public static InMemorySink Instance
        {
            get
            {
                if (LocalInstance.Value == null)
                {
                    LocalInstance.Value = new InMemorySink();
                }

                return LocalInstance.Value;
            }
        }

        public IEnumerable<LogEvent> LogEvents => _logEvents.AsReadOnly();

        public void Emit(LogEvent logEvent)
        {
            _logEvents.Add(logEvent);
        }

        public void Dispose()
        {
            _logEvents.Clear();
        }
    }
}