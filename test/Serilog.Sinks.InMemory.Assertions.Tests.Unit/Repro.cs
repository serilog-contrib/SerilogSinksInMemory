using System;
using FluentAssertions;
using Serilog.Events;
using Xunit;
using Serilog.Sinks.InMemory.Assertions;

namespace Serilog.Sinks.InMemory.Assertions.Tests.Unit
{
public class Repro
{
    [Fact]
    public void FromIssue22()
    {
        var logger = new LoggerConfiguration()
            .WriteTo.InMemory()
            .CreateLogger();

        logger.Error(
            new ArgumentException("Account name cannot be longer than 100 characters"),
            "An invalid argument was encountered while processing message with ID {MessageId}", "some-message-id");
        
        InMemorySink.Instance.Should()
            .HaveErrorMessageWithException<ArgumentException>(
                "An invalid argument was encountered while processing message with ID {MessageId}",
                "Account name cannot be longer than 100 characters");
    }
}

public static class Extension
{
    public static void HaveErrorMessageWithException<T>(this InMemorySinkAssertions assertion, string messageTemplate, string innerMessage = null)
    {
        if (innerMessage == null)
        {
            assertion.HaveMessage(messageTemplate)
                .WithLevel(LogEventLevel.Error)
                .Appearing().Once()
                .Match(o => o.Exception.GetType() == typeof(T));
        }

        assertion.HaveMessage(messageTemplate)
            .WithLevel(LogEventLevel.Error)
            .Appearing().Once()
            .Match(o => o.Exception.GetType() == typeof(T) &&
                    o.Exception.Message.Contains(innerMessage)
            );
    }
}
}
