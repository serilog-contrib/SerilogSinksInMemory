using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using Serilog.Core;

namespace Serilog.Sinks.InMemory.Tests.Unit
{
    public class WhenConfiguringInMemorySink
    {
        [Test]
        public void GivenConfigurationToWriteToInMemorySink_InMemorySinkIsAddedToLogger()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .CreateLogger();

            // Because there is no way to get access to the sinks configured for a logger
            // we need to use reflection to get at it...

            // The first one is a SafeAggregatedSink which is internal
            var instance = logger
                .GetType()
                .GetField("_sink", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(logger);

            // It has a field containing all configured sinks
            var sinks = (ILogEventSink[])instance
                .GetType()
                .GetField("_sinks", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(instance);

            sinks.Should().Contain(s => s.GetType() == typeof(InMemorySink));
        }
    }
}
