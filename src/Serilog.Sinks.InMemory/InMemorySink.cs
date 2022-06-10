using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.InMemory
{
    public class InMemorySink : ILogEventSink, IDisposable
    {
        private static readonly AsyncLocal<InMemorySink> LocalInstance = new AsyncLocal<InMemorySink>();

        private readonly List<LogEvent> _logEvents;
        private readonly object _snapShotLock = new object();

        public InMemorySink() : this(new List<LogEvent>())
        {
        }

        protected InMemorySink(List<LogEvent> logEvents)
        {
            _logEvents = logEvents;
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

        public void Dispose()
        {
            _logEvents.Clear();
        }

        public virtual void Emit(LogEvent logEvent)
        {
            lock (_snapShotLock)
            {
                _logEvents.Add(logEvent);
            }
        }

        public InMemorySink Snapshot()
        {
            lock (_snapShotLock)
            {
                var currentLogEvents = _logEvents.AsReadOnly().ToList();

                return new InMemorySinkSnapshot(currentLogEvents);
            }
        }
    }
}