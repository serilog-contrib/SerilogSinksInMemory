namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

public sealed partial class InMemorySinkAssertionsFactoryImpl : InMemorySinkAssertionsFactory
{
    public AssertionFramework AssertionFramework => AssertionExtensions.AssertionFramework;
}