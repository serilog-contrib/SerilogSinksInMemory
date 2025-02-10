#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Serilog.Sinks.InMemory.Assertions
{
    public static class InMemorySinkAssertionExtensions
    {
        private static Type? _assertionsType;
        private static readonly object SyncRoot = new();

        public static InMemorySinkAssertions Should(this InMemorySink instance)
        {
            if (_assertionsType == null)
            {
                lock (SyncRoot)
                {
                    var assemblyLocation =
                        Path.GetDirectoryName(typeof(InMemorySinkAssertionExtensions).Assembly.Location);

                    if (string.IsNullOrEmpty(assemblyLocation))
                    {
                        throw new Exception($"Unable to determine path to load assemblies from");
                    }

                    var fluentAssertionsAssembly = AppDomain
                        .CurrentDomain
                        .GetAssemblies()
                        .FirstOrDefault(assembly => assembly.GetName().Name.Equals("FluentAssertions"));

                    if (fluentAssertionsAssembly == null)
                    {
                        var fluentAssertionsAssemblyPath = Path.Combine(assemblyLocation, "FluentAssertions.dll");

                        try
                        {
                            fluentAssertionsAssembly = Assembly.LoadFile(fluentAssertionsAssemblyPath);
                        }
                        catch (FileNotFoundException e)
                        {
                            throw new Exception($"Could not find assembly '{fluentAssertionsAssemblyPath}'", e);
                        }
                    }

                    var assertionLibrary = IsAwesomeAssertions(fluentAssertionsAssembly)
                        ? "AwesomeAssertions"
                        : "FluentAssertions";

                    var fluentAssertionsMajorVersion = fluentAssertionsAssembly.GetName().Version.Major;

                    var versionedLocation = Path.Combine(
                        assemblyLocation,
                        $"Serilog.Sinks.InMemory.{assertionLibrary}{fluentAssertionsMajorVersion}.dll");

                    var versionedAssembly = Assembly.LoadFile(versionedLocation);

                    _assertionsType = versionedAssembly
                        .GetTypes()
                        .SingleOrDefault(t => t.Name == "InMemorySinkAssertionsImpl");
                }
            }

            if (_assertionsType == null)
            {
                throw new InvalidOperationException("Unable to load InMemorySinkAssertions");
            }

            var snapshotInstance = SnapshotOf(instance);
            
            return (InMemorySinkAssertions)Activator.CreateInstance(
                _assertionsType, snapshotInstance);
        }

        private static bool IsAwesomeAssertions(Assembly fluentAssertionsAssembly)
        {
            var assemblyMetadata = fluentAssertionsAssembly.GetCustomAttributes<AssemblyMetadataAttribute>();

            // Yuck yuck yuck
            return assemblyMetadata.Any(metadata => metadata.Value.Contains("AwesomeAssertions"));
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