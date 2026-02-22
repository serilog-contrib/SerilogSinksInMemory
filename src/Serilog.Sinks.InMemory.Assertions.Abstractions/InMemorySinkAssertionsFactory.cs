namespace Serilog.Sinks.InMemory.Assertions;

public interface InMemorySinkAssertionsFactory
{
    AssertionFramework AssertionFramework { get; }
    InMemorySinkAssertions CreateInMemorySinkAssertions(InMemorySink snapshotInstance);
}