using System;
using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Sinks.InMemory
{
    /// <summary>
    /// A snapshot of an InMemorySink instance that is used for assertions
    /// </summary>
    internal class InMemorySinkSnapshot : InMemorySink
    {
        public InMemorySinkSnapshot(List<LogEvent> logEvents) : base(logEvents)
        {
        }

        public override void Emit(LogEvent logEvent)
        {
            throw new InvalidOperationException("Can't write log events to a in-memory sink snapshot because it is a read-only representation");
        }
    }
}