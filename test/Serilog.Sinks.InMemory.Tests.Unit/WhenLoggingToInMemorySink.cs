using System;
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

        [Fact]
        public void GivenRestrictedMinimumLevelMessagesAreWritten_FilteredLogEventsAreStoredInSink()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.InMemory(restrictedToMinimumLevel: Events.LogEventLevel.Information)
                .CreateLogger();

            logger.Verbose("Verbose Message");
            logger.Debug("Debug Message");
            logger.Information("Information Message");

            InMemorySink.Instance
                .LogEvents
                .Should()
                .HaveCount(1);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void GivenSnapshotIsTaken_SnapshotDoesNotIncludeEventsLoggedAfterSnapshot()
        {
            var sink = new InMemorySink();
            var logger = new LoggerConfiguration()
                .WriteTo.Sink(sink)
                .CreateLogger();

            logger.Information("First");
            logger.Information("Second");
            var snapshot = sink.Snapshot();
            logger.Information("Third");

            snapshot.LogEvents.Should().HaveCount(2);
            sink.LogEvents.Should().HaveCount(3);
        }

        [Fact]
        public void GivenDisposeIsCalledTwice_DoesNotThrow()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .CreateLogger();

            logger.Information("Test");

            Action act = () =>
            {
                logger.Dispose();
                logger.Dispose();
            };

            act.Should().NotThrow();
            InMemorySink.Instance.LogEvents.Should().BeEmpty();
        }
    }
}