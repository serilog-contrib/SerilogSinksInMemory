using Serilog.Configuration;

namespace Serilog.Sinks.InMemory
{
    public static class InMemorySinkExtensions
    {
        public static LoggerConfiguration InMemory(
            this LoggerSinkConfiguration loggerConfiguration)
        {
            return loggerConfiguration.Sink(new InMemorySink());
        }
    }
}
