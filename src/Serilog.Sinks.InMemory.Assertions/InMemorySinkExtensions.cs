using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Serilog.Sinks.InMemory.Assertions.Abstractions;

namespace Serilog.Sinks.InMemory.Assertions
{
    public static class InMemorySinkAssertionExtensions
    {
        private static Type? AssertionsType = null;
        private static readonly object SyncRoot = new();

        public static InMemorySinkAssertions Should(this InMemorySink instance)
        {
            if (AssertionsType != null)
            {
                var snapshotInstance = SnapshotOf(instance);
                return (InMemorySinkAssertions)Activator.CreateInstance(AssertionsType,
                    new object[] { snapshotInstance });
            }

            lock (SyncRoot)
            {
                var assemblyLocation = Path.GetDirectoryName(typeof(InMemorySink).Assembly.Location);
                var fluentAssertionsAssemblyPath = Path.Combine(assemblyLocation, "FluentAssertions.dll");
                var fluentAssertionsAssembly = Assembly.LoadFile(fluentAssertionsAssemblyPath);
                var fluentAssertionsMajorVersion = fluentAssertionsAssembly.GetName().Version.Major;

                var versionedLocation = Path.Combine(
                    assemblyLocation,
                        $"Serilog.Sinks.InMemory.FluentAssertions{fluentAssertionsMajorVersion}.dll");
                
                var versionedAssembly = Assembly.LoadFile(versionedLocation);

                try
                {
                    AssertionsType = versionedAssembly.GetTypes()
                        .SingleOrDefault(t => t.Name == "InMemorySinkAssertionsImpl");
                }
                catch (ReflectionTypeLoadException e)
                {
                    Debugger.Break();
                    throw;
                }

                if (AssertionsType == null)
                {
                    throw new InvalidOperationException("Unable to load InMemorySinkAssertions");
                }

                var snapshotInstance = SnapshotOf(instance);
                return (InMemorySinkAssertions)Activator.CreateInstance(AssertionsType,
                    new object[] { snapshotInstance });
            }
        }

        /*
         * Hack attack.
         *
         * This is a bit of a dirty way to work around snapshotting the InMemorySink instance
         * to ensure that you won't get hit by an InvalidOperationException when calling
         * HaveMessage() and the logger gets called from somewhere else and adds a new
         * LogEvent to the collection while that method is invoked.
         *
         * For now we copy the LogEvents from the current sink and use reflection to assign
         * it to a new instance of InMemorySink that will be used by the assertions,
         * effectively creating a snapshot of the InMemorySink that was used by the tests.
         */
        private static InMemorySink SnapshotOf(InMemorySink instance)
        {
            return instance.Snapshot();
        }
    }
}