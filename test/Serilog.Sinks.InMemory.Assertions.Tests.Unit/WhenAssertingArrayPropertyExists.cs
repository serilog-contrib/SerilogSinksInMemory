using System;
#if AWESOMEASSERTIONS_9
using AwesomeAssertions;
#else
using FluentAssertions;
#endif
using Serilog.Context;
using Xunit;
using Serilog.Sinks.InMemory.Assertions;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
    public class WhenAssertingArrayPropertyExists
    {
        private readonly ILogger _logger;

        public WhenAssertingArrayPropertyExists()
        {
            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.InMemory()
                .CreateLogger();
        }

        [Fact]
        public void GivenArrayPropertyAndAssertingValue_HavePropertySucceeds()
        {
            var ids = new long[] { 1, 2 };
            _logger.Information("Ids {ids}", ids);

            InMemorySink.Instance
                .Should()
                .HaveMessage("Ids {ids}")
                .Appearing().Once()
                .WithProperty("ids")
                .WithValue(ids);
        }

        [Fact]
        public void GivenArrayPropertyAndAssertingDifferentOrder_HavePropertyFails()
        {
            var ids = new long[] { 2, 1 };
            _logger.Information("Ids {ids}", ids);

            Action action = () => InMemorySink.Instance
                .Should()
                .HaveMessage("Ids {ids}")
                .Appearing().Once()
                .WithProperty("ids")
                .WithValue(new long[] { 1, 2 });

            action
                .Should()
                .Throw<Exception>();
        }

        [Fact]
        public void GivenEmptyArrayProperty_HavePropertySucceeds()
        {
            var ids = Array.Empty<long>();
            _logger.Information("Ids {ids}", ids);

            InMemorySink.Instance
                .Should()
                .HaveMessage("Ids {ids}")
                .Appearing().Once()
                .WithProperty("ids")
                .WithValue(ids);
        }

        [Fact]
        public void GivenNullableArrayProperty_HavePropertySucceeds()
        {
            var ids = new int?[] { 1, null, 3 };
            _logger.Information("Ids {ids}", ids);

            InMemorySink.Instance
                .Should()
                .HaveMessage("Ids {ids}")
                .Appearing().Once()
                .WithProperty("ids")
                .WithValue(ids);
        }
    }
}
