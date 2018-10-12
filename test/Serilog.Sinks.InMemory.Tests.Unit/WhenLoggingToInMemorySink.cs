using FluentAssertions;
using Xunit;

namespace Serilog.Sinks.InMemory.Tests.Unit
{
    public class WhenLoggingToInMemorySink
    {
        [Fact]
        public void GivenInformationMessageIsWritten_LogEventIsStoredInSink()
        {
            var sink = new InMemorySink();

            var logger = new LoggerConfiguration()
                .WriteTo.Sink(sink)
                .CreateLogger();

            logger.Information("Test");

            sink
                .LogEvents
                .Should()
                .HaveCount(1);
        }
    }
}