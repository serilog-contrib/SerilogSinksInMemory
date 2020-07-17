using System;
using System.Collections.Generic;
using System.Threading;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.InMemory
{
    public class InMemorySink : ILogEventSink, IDisposable
    {
        private static readonly AsyncLocal<InMemorySink> LocalInstance = new AsyncLocal<InMemorySink>();
        private readonly ITextFormatter _formatProvider;

        private readonly List<LogEvent> _logEvents;

        public InMemorySink()
        {
            _logEvents = new List<LogEvent>();
        }

        public InMemorySink(ITextFormatter formatProvider)
        {
            _formatProvider = formatProvider;
            _logEvents = new List<LogEvent>();
        }

        public static InMemorySink Instance {
            get {
                if (LocalInstance.Value == null)
                {
                    LocalInstance.Value = new InMemorySink();
                }
                return LocalInstance.Value;
            }
        }


        public static InMemorySink GetInstance(ITextFormatter formatProvider = null)
        {
            if (LocalInstance.Value == null)
            {
                LocalInstance.Value = new InMemorySink(formatProvider);
            }

            return LocalInstance.Value;
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