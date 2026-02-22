#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.InMemory
{
    [DebuggerTypeProxy(typeof(InMemorySinkAdvDebugProxy))]
    public class InMemorySink : ILogEventSink, IDisposable
    {
        private static readonly AsyncLocal<InMemorySink> LocalInstance = new AsyncLocal<InMemorySink>();

        private ReadOnlyCollection<LogEvent>? _logEventsSnapshot;
        private readonly List<LogEvent> _logEvents;
        private readonly object _snapShotLock = new object();

        public InMemorySink()
            : this(new List<LogEvent>())
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
                LocalInstance.Value ??= new InMemorySink();
                return LocalInstance.Value;
            }
        }

        public IEnumerable<LogEvent> LogEvents => GetLogEvents();

        public void Dispose()
        {
            _logEvents.Clear();
        }

        public virtual void Emit(LogEvent logEvent)
        {
            lock (_snapShotLock)
            {
                _logEvents.Add(logEvent);
                _logEventsSnapshot = null;
            }
        }

        private IEnumerable<LogEvent> GetLogEvents()
        {
            if (_logEventsSnapshot == null)
            {
                lock (_snapShotLock)
                {
                    _logEventsSnapshot ??= _logEvents.AsReadOnly();
                }
            }

            return _logEventsSnapshot;
        }

        public InMemorySink Snapshot()
        {
            var currentLogEvents = GetLogEvents().ToList();
            return new InMemorySinkSnapshot(currentLogEvents);
        }

        public InMemorySink Snapshot(Func<LogEvent, bool> predicate)
        {
            var currentLogEvents = GetLogEvents().Where(predicate).ToList();
            return new InMemorySinkSnapshot(currentLogEvents);
        }
    }
}