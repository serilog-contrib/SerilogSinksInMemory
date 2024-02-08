namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit;

public class WhenAssertingAndSInkIsWrittenTo
{
    [Fact]
    public void AssertionShouldNotFailWhenInstanceIsLoggedToAfterInvokingTheAssertion()
    {
        /*
         * This test is meant to verify the behaviour of the assertions
         * when a message is logged to the instance we're asserting on
         * after we invoked the assertion.
         *
         * The behaviour is that when calling Should(), the assertion
         * should (pun intended) capture a snapshot of the log events
         * at the point in time it was invoked.
         *
         * This prevents InvalidOperationExceptions because the internal
         * collection is modified.
         *
         * See https://github.com/sandermvanvliet/SerilogSinksInMemory/issues/16
         */
        var sink = new InMemorySink();

        var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .CreateLogger();

        // Log 2 messages
        logger.Information("Message");
        logger.Information("Message");

        // Start assertion
        var assertion = sink.Should();

        // Pretend another thread/task/fiber logs another message
        logger.Information("Message");

        // Continue with the assertion
        assertion
            .HaveMessage("Message")
            .Appearing().Times(2);
    }
}
