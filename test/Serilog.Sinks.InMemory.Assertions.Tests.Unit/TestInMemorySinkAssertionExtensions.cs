using Serilog.Sinks.InMemory.Assertions;

namespace Serilog.Sinks.InMemory.AssertionsTests;

public static class TestInMemorySinkAssertionExtensions
{
    public static InMemorySinkAssertionsFactory AssertionsFactory { get; }

    static TestInMemorySinkAssertionExtensions()
    {
        var (assertionFramework, majorVersion) = GetAssertionsFramework();
        AssertionsFactory = InMemorySinkAssertionUtils.CreateMemorySinkAssertionsFactory(assertionFramework, majorVersion);
    }

    private static (AssertionFrameworks assertionFramework, int majorVersion) GetAssertionsFramework()
    {
#if AWESOMEASSERTIONS_8
        return (AssertionFrameworks.AwesomeAssertions, 8);
#elif AWESOMEASSERTIONS_9
        return (AssertionFrameworks.AwesomeAssertions, 9);
#elif FLUENTASSERTIONS_5
        return (AssertionFrameworks.FluentAssertions, 5);
#elif FLUENTASSERTIONS_6
        return (AssertionFrameworks.FluentAssertions, 6);
#elif FLUENTASSERTIONS_7
        return (AssertionFrameworks.FluentAssertions, 7);
#elif FLUENTASSERTIONS_8
        return (AssertionFrameworks.FluentAssertions, 8);
#elif SHOULDLY_4
        return (AssertionFrameworks.Shouldly, 4);
#else
        #error Unsupported assertion framework
#endif
    }

    public static InMemorySinkAssertions Should(this InMemorySink instance) 
        => AssertionsFactory.CreateInMemorySinkAssertionsFromSnapshot(instance);
}