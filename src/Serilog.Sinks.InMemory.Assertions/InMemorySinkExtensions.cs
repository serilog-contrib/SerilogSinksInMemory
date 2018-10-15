namespace Serilog.Sinks.InMemory.Assertions
{
    public static class InMemorySinkAssertionExtensions
    {
        public static InMemorySinkAssertions Should(this InMemorySink instance)
        {
            return new InMemorySinkAssertions(instance);
        }
    }
}
