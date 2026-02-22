#nullable enable
namespace Serilog.Sinks.InMemory.Assertions;

public readonly struct AssertionExtension<T>
{
    private readonly IInMemorySinkAssertionsExtension _extension;

    public AssertionExtension(T assertions, IInMemorySinkAssertionsExtension extension)
    {
        _extension = extension;
        Assertions = assertions;
    }

    public T Assertions { get; }

    public AssertionFramework AssertionFramework => _extension.AssertionFramework;

    public void Assert(bool condition, FailMessage failureMessage, string because = "", params object[] becauseArgs)
    {
        _extension.Assert(condition, failureMessage, because, becauseArgs);
    }
}