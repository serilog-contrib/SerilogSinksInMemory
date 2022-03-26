using System;
using System.Net.Sockets;
using FluentAssertions;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingLogEventWithException
    {
        private readonly Logger _logger;
        private readonly InMemorySink _inMemorySink;

        public WhenAssertingLogEventWithException()
        {
            _inMemorySink = new InMemorySink();

            _logger = new LoggerConfiguration()
                .WriteTo.Sink(_inMemorySink)
                .CreateLogger();
        }

        [Fact]
        public void GivenLogMessageWithException_OldVerboseStyle()
        {
            GivenLogMessaageWithException();

            _inMemorySink
                .Should()
                .HaveMessage("BANG!")
                .Appearing()
                .Once()
                .WithLevel(LogEventLevel.Error)
                .Subject
                .Exception
                .Message
                .Should()
                .Be("BANG BANG!");
        }

        [Fact]
        public void GivenLogMessageWithException_NewStyle()
        {
            GivenLogMessaageWithException();

            _inMemorySink
                .Should()
                .HaveMessage("BANG!")
                .Appearing()
                .Once()
                .WithException<Exception>()
                .AndMessage("BANG BANG!");
        }

        [Fact]
        public void GivenLogMessageWithException_NewStyleFullMatch()
        {
            GivenLogMessaageWithException();

            var exception = new Exception("BANG BANG!");
            exception.Data.Add("some", "property");

            _inMemorySink
                .Should()
                .HaveMessage("BANG!")
                .Appearing()
                .Once()
                .WithException<Exception>();
        }

        [Fact]
        public void GivenLogMessageWithException_NewStyleAndInnerException()
        {
            GivenLogMessaageWithException();

            _inMemorySink
                .Should()
                .HaveMessage("BANG!")
                .Appearing()
                .Once()
                .WithException<Exception>()
                .AndInnerException<ArgumentException>()
                .Which
                .Message
                .Should()
                .Be("INNER BANG BANG!");
        }

        [Fact]
        public void GivenLogMessageWithAggregateException_NewStyleAndInnerException()
        {
            var exceptionOne = new ArgumentException("BANG 1!");
            var exceptionTwo = new InvalidOperationException("BANG 2!");
            var exceptionThree = new ApplicationException("BANG 3!");

            var aggregateException = new AggregateException(
                exceptionOne,
                exceptionTwo,
                exceptionThree);

            _logger.Error(aggregateException, "BANG!");

            _inMemorySink
                .Should()
                .HaveMessage("BANG!")
                .Appearing()
                .Once()
                .WithException<AggregateException>()
                .ContainingException(e => e is InvalidOperationException);
        }

        private void GivenLogMessaageWithException()
        {
            var exception = new Exception(
                "BANG BANG!", 
                new ArgumentException("INNER BANG BANG!"));

            exception.Data.Add("some", "property");

            _logger.Error(exception, "BANG!");
        }
    }
}