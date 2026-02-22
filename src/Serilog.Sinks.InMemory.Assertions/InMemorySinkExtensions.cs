#nullable enable

namespace Serilog.Sinks.InMemory.Assertions;

public static class InMemorySinkAssertionExtensions
{
    public static InMemorySinkAssertionsFactory AssertionsFactory { get; }

    static InMemorySinkAssertionExtensions()
    {
        var (assertionFramework, majorVersion, assemblyLocation) = InMemorySinkAssertionUtils.GetAssertionsFramework();
        AssertionsFactory = InMemorySinkAssertionUtils.CreateMemorySinkAssertionsFactory(assertionFramework, majorVersion, assemblyLocation);
    }

    public static InMemorySinkAssertions Should(this InMemorySink instance)
        => AssertionsFactory.CreateInMemorySinkAssertionsFromSnapshot(instance);
}