#nullable enable

namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

[ShouldlyMethods]
partial class InMemorySinkAssertionsImpl : BaseShouldlyAssertions<InMemorySink>
{
    public InMemorySinkAssertionsImpl(InMemorySink snapshotInstance)
        : base(snapshotInstance)
    {
    }
}