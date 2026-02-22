#nullable enable
namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

partial class InMemorySinkAssertionsImpl : BaseAssertions<InMemorySink, InMemorySinkAssertionsImpl>
{
    public InMemorySinkAssertionsImpl(InMemorySink snapshotInstance)
        : base(snapshotInstance, AssertionChain.GetOrCreate())
    {
    }

    protected override string Identifier => nameof(InMemorySink);
}