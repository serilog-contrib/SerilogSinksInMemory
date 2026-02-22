#nullable enable
namespace Serilog.Sinks.InMemory.Assertions;

public readonly struct FailMessage
{
    public static implicit operator FailMessage(string value)
    {
        return new FailMessage(value);
    }

    public FailMessage(string message, params object?[] args)
    {
        Message = message;
        Args = args;
    }

    public string Message { get; }
    public object?[] Args { get; }
}