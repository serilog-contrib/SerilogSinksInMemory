using System;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingMessageExistsThatContainsPattern
    {
        [Fact]
        public void GivenThreeMessagesWithPattern_AssertionSucceeds()
        {
            _logger.Information("padding pattern padding");
            _logger.Information("pattern padding");
            _logger.Information("padding pattern");

            InMemorySink.Instance
                .Should()
                .HaveMessage()
                .Containing("pattern")
                .Appearing().Times(3);
        }

        [Fact]
        public void GivenThreeMessagesNotMatchingPattern_AssertionFails()
        {
            _logger.Information("padding pattern padding");
            _logger.Information("pattern padding");
            _logger.Information("padding pattern");

            Action action = () => InMemorySink.Instance
                .Should()
                .HaveMessage()
                .Containing("NOT MATCHING")
                .Appearing().Times(3);

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected a message with pattern \"NOT MATCHING\" to be logged");
        }

        [Fact]
        public void GivenThreeMessagesAndAssertingAppearanceCount_AssertionSucceeds()
        {
            _logger.Information("foo");
            _logger.Information("bar");
            _logger.Information("baz");

            InMemorySink.Instance
                .Should()
                .HaveMessage()
                .Appearing().Times(3);
        }

        [Fact]
        public void GivenThreeMessagesAndAssertingNoMessagesAreLogged_AssertionFails()
        {
            _logger.Information("foo");
            _logger.Information("bar");
            _logger.Information("baz");

            Action action = () => InMemorySink.Instance
                .Should()
                .NotHaveMessage();

            action
                .Should()
                .Throw<XunitException>()
                .WithMessage("Expected no messages to be logged, but found 3 messages");
        }

        private readonly ILogger _logger;

        public WhenAssertingMessageExistsThatContainsPattern()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .CreateLogger();
        }
    }
}
