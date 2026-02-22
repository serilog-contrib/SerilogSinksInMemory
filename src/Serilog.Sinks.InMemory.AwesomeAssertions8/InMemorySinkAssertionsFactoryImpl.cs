namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

partial class InMemorySinkAssertionsFactoryImpl
{
    public InMemorySinkAssertions CreateInMemorySinkAssertions(InMemorySink snapshotInstance)
        => new InMemorySinkAssertionsImpl(snapshotInstance);
}