using FluentAssertions;
using Xunit;

namespace Serilog.Sinks.InMemory.Tests.Unit
{
    public class WhenLoggingToInMemorySink
    {
        [Fact]
        public void GivenInformationMessageIsWritten_LogEventIsStoredInSink()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .CreateLogger();

            logger.Information("Test");

            InMemorySink.Instance
                .LogEvents
                .Should()
                .HaveCount(1);
        }
    }
}