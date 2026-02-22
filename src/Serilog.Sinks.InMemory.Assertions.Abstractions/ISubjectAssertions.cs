namespace Serilog.Sinks.InMemory.Assertions;

public interface ISubjectAssertions<out T>
{
    T Subject { get; }
}