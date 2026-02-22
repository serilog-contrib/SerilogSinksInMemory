#nullable enable
namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

public abstract class BaseAssertions<TSubject, TAssertions> : ReferenceTypeAssertions<TSubject, TAssertions>, IInMemorySinkAssertionsExtension
    where TAssertions : BaseAssertions<TSubject, TAssertions>
{
    protected BaseAssertions(TSubject subject)
        : base(subject)
    {
    }

    public AssertionFramework AssertionFramework => AssertionExtensions.AssertionFramework;
    public void Assert(bool condition, FailMessage failureMessage, string because = "", params object[] becauseArgs)
        => failureMessage.Assert(condition, because: because, becauseArgs: becauseArgs);
}