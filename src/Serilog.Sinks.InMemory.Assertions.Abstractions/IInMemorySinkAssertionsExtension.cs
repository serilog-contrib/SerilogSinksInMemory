#nullable enable
namespace Serilog.Sinks.InMemory.Assertions;

public interface IInMemorySinkAssertionsExtension
{
    void Assert(
        bool condition,
        FailMessage failureMessage,
        string because = "",
        params object[] becauseArgs);

    AssertionFramework AssertionFramework { get; }
}