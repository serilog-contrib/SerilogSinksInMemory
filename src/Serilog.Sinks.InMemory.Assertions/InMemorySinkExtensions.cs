#nullable enable
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

                    string? adapterName = null;
                    int? majorVersion = null;
                    Assembly? versionedAssembly = null;

                    // Order is important here, first check the loaded assemblies before
                    // looking on disk because otherwise we might load FluentAssertions from disk
                    // while Shouldly is already loaded into the AppDomain and that's the one we
                    // should be using.
                    // That's also a guess but hey, if you mix and match assertion frameworks you
                    // can deal with the fall out.
                    if (IsFluentAssertionsAlreadyLoadedIntoDomain(out var fluentAssertionsAssembly))
                    {
                        adapterName = "FluentAssertions";
                        majorVersion = fluentAssertionsAssembly.GetName().Version.Major;
                    }
                    else if (IsAwesomeAssertionsAlreadyLoadedIntoDomain(out var awesomeAssertionsAssembly))
                    {
                        adapterName = "AwesomeAssertions";
                        majorVersion = awesomeAssertionsAssembly.GetName().Version.Major;
                    }
                    else if (IsShouldlyAlreadyLoadedIntoDomain(out var shouldlyAssembly))
                    {
                        adapterName = "Shouldly";
                        majorVersion = shouldlyAssembly.GetName().Version.Major;
                    }
                    else if (IsFluentAssertionsAvailableOnDisk(assemblyLocation,
                                 out var fluentAssertionsOnDiskAssembly))
                    {
                        adapterName = "FluentAssertions";
                        majorVersion = fluentAssertionsOnDiskAssembly.GetName().Version.Major;
                    }
                    else if (IsAwesomeAssertionsAvailableOnDisk(assemblyLocation,
                                 out var awesomeAssertionsOnDiskAssembly))
                    {
                        adapterName = "AwesomeAssertions";
                        majorVersion = awesomeAssertionsOnDiskAssembly.GetName().Version.Major;
                    }
                    else if (IsShouldlyAvailableOnDisk(assemblyLocation, out var shouldlyOnDiskAssembly))
                    {
                        adapterName = "Shouldly";
                        majorVersion = shouldlyOnDiskAssembly.GetName().Version.Major;
                    }

                    if (adapterName != null && majorVersion != null)
                    {
                        var versionedLocation = Path.Combine(
                            assemblyLocation,
                            $"Serilog.Sinks.InMemory.{adapterName}{majorVersion}.dll");

                        if (!File.Exists(versionedLocation))
                        {
                            throw new InvalidOperationException($"Detected {adapterName} version {majorVersion} but the assertions adapter wasn't found on disk");
                        }
                        
                        versionedAssembly = Assembly.LoadFile(versionedLocation);
                    }

                    if (versionedAssembly != null)
                    {
                        _assertionsType = versionedAssembly
                            .GetTypes()
                            .SingleOrDefault(t => t.Name == "InMemorySinkAssertionsImpl");
                    }
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

        private static bool IsFluentAssertionsAlreadyLoadedIntoDomain(
            [NotNullWhen(true)] out Assembly? fluentAssertionsAssembly)
        {
            fluentAssertionsAssembly = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(assembly =>
                {
                    if (assembly.GetName().Name.Equals("FluentAssertions"))
                    {
                        var metadataAttributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToArray();

                        return !metadataAttributes.Any() ||
                               metadataAttributes.Any(metadata => metadata.Value.Contains("FluentAssertions", StringComparison.OrdinalIgnoreCase));
                    }

                    return false;
                });

            return fluentAssertionsAssembly != null;
        }

        private static bool IsAwesomeAssertionsAlreadyLoadedIntoDomain(
            [NotNullWhen(true)] out Assembly? awesomeAssertionsAssembly)
        {
            awesomeAssertionsAssembly = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(assembly =>
                    assembly.GetName().Name.Equals("FluentAssertions") &&
                    assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                        .Any(metadata => metadata.Value.Contains("AwesomeAssertions", StringComparison.OrdinalIgnoreCase)));

            return awesomeAssertionsAssembly != null;
        }

        private static bool IsShouldlyAlreadyLoadedIntoDomain([NotNullWhen(true)] out Assembly? shouldlyAssembly)
        {
            shouldlyAssembly = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(assembly => assembly.GetName().Name.Equals("Shouldly"));

            return shouldlyAssembly != null;
        }

        private static bool IsFluentAssertionsAvailableOnDisk(
            string assemblyLocation,
            [NotNullWhen(true)] out Assembly? assembly)
        {
            var assemblyPath = Path.Combine(assemblyLocation, "FluentAssertions.dll");

            if (File.Exists(assemblyPath))
            {
                assembly = Assembly.LoadFile(assemblyPath);

                var metadataAttributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToList();
                
                if(!metadataAttributes.Any() || metadataAttributes.Any(metadata => metadata.Value.Contains("FluentAssertions", StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }

            assembly = null;
            return false;
        }

        private static bool IsAwesomeAssertionsAvailableOnDisk(
            string assemblyLocation,
            [NotNullWhen(true)] out Assembly? assembly)
        {
            var assemblyPath = Path.Combine(assemblyLocation, "FluentAssertions.dll");

            if (File.Exists(assemblyPath))
            {
                assembly = Assembly.LoadFile(assemblyPath);

                if (assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                    .Any(metadata => metadata.Value.Contains("AwesomeAssertions", StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }

            assembly = null;
            return false;
        }

        private static bool IsShouldlyAvailableOnDisk(
            string assemblyLocation,
            [NotNullWhen(true)] out Assembly? assembly)
        {
            var assemblyPath = Path.Combine(assemblyLocation, "Shouldly.dll");

            if (File.Exists(assemblyPath))
            {
                assembly = Assembly.LoadFile(assemblyPath);
                return true;
            }

            assembly = null;
            return false;
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