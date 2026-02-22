namespace Serilog.Sinks.InMemory.AssertionsFrameworkExtension;

public abstract class BaseShouldlyAssertions<T> : ISubjectAssertions<T>
{
    protected BaseShouldlyAssertions(T subject)
    {
        Subject = subject;
    }

    public T Subject { get; }

    public AssertionFramework AssertionFramework => AssertionExtensions.AssertionFramework;

    public void Assert([DoesNotReturnIf(false)] bool condition, FailMessage failureMessage, string because = "", params object[] becauseArgs) =>
        failureMessage.Assert(condition, because, becauseArgs);
}