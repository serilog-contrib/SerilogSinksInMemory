using FluentAssertions;
using NUnit.Framework;

namespace Serilog.Sinks.InMemory.Tests.Unit
{
    public class WhenLoggingToInMemorySink
    {
        [Test]
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

        [Test]
        public void GivenLoggerIsDisposed_LogEventsAreCleared()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .CreateLogger();

            logger.Information("Test");

            logger.Dispose();

            InMemorySink.Instance
                .LogEvents
                .Should()
                .HaveCount(0);
        }

        [Test]
        public void GivenLoggerIsDisposedAndNewMessageIsLogged_SinkOnlyContainsSecondMessage()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .CreateLogger();

            logger.Information("First");

            logger.Dispose();

            logger.Information("Second");

            InMemorySink.Instance
                .LogEvents
                .Should()
                .OnlyContain(l => l.MessageTemplate.Text == "Second");
        }
    }
}