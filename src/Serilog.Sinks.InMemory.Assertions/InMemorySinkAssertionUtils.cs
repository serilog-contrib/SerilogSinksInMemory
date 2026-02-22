#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Serilog.Sinks.InMemory.Assertions;

public static class InMemorySinkAssertionUtils
{
    public static InMemorySinkAssertionsFactory CreateMemorySinkAssertionsFactory(
        AssertionFrameworks assertionFramework,
        int majorVersion,
        string? assemblyLocation = null)
    {
        var factoryType = GetAssertionsFactoryType(assertionFramework, majorVersion, assemblyLocation) ??
                          throw new InvalidOperationException("Unable to load InMemorySinkAssertionsFactory");

        return (InMemorySinkAssertionsFactory)Activator.CreateInstance(factoryType);
    }

    public static InMemorySinkAssertions CreateInMemorySinkAssertionsFromSnapshot(this InMemorySinkAssertionsFactory factory, InMemorySink instance)
        => factory.CreateInMemorySinkAssertions(instance.Snapshot());

    internal static (AssertionFrameworks Framework, int MajorVersion, string AssemblyLocation) GetAssertionsFramework()
    {
        var assemblyLocation = GetInMemoryExtensionsAssemblyLocation();

        AssertionFrameworks? assertionFramework = null;
        int? majorVersion = null;

        // Order is important here, first check the loaded assemblies before
        // looking on disk because otherwise we might load FluentAssertions from disk
        // while Shouldly is already loaded into the AppDomain and that's the one we
        // should be using.
        // That's also a guess but hey, if you mix and match assertion frameworks you
        // can deal with the fall out.
        if (IsFluentAssertionsAlreadyLoadedIntoDomain(out var fluentAssertionsAssembly))
        {
            assertionFramework = AssertionFrameworks.FluentAssertions; // "FluentAssertions"
            majorVersion = fluentAssertionsAssembly.GetName().Version.Major;
        }
        else if (IsAwesomeAssertionsAlreadyLoadedIntoDomain(out var awesomeAssertionsAssembly))
        {
            assertionFramework = AssertionFrameworks.AwesomeAssertions;
            majorVersion = awesomeAssertionsAssembly.GetName().Version.Major;
        }
        else if (IsShouldlyAlreadyLoadedIntoDomain(out var shouldlyAssembly))
        {
            assertionFramework = AssertionFrameworks.Shouldly;
            majorVersion = shouldlyAssembly.GetName().Version.Major;
        }
        else if (IsFluentAssertionsAvailableOnDisk(assemblyLocation, out var fluentAssertionsOnDiskAssembly))
        {
            assertionFramework = AssertionFrameworks.FluentAssertions;
            majorVersion = fluentAssertionsOnDiskAssembly.GetName().Version.Major;
        }
        else if (IsAwesomeAssertionsAvailableOnDisk(assemblyLocation,
                     out var awesomeAssertionsOnDiskAssembly))
        {
            assertionFramework = AssertionFrameworks.AwesomeAssertions;
            majorVersion = awesomeAssertionsOnDiskAssembly.GetName().Version.Major;
        }
        else if (IsShouldlyAvailableOnDisk(assemblyLocation, out var shouldlyOnDiskAssembly))
        {
            assertionFramework = AssertionFrameworks.Shouldly;
            majorVersion = shouldlyOnDiskAssembly.GetName().Version.Major;
        }

        if (assertionFramework == null || majorVersion == null)
        {
            throw new InvalidOperationException("Unable to determine assertion framework to use");
        }

        return (assertionFramework.Value, majorVersion.Value, assemblyLocation);
    }

    private static string GetInMemoryExtensionsAssemblyLocation()
    {
        var assemblyLocation = Path.GetDirectoryName(typeof(InMemorySinkAssertionUtils).Assembly.Location);
        return string.IsNullOrEmpty(assemblyLocation) ? throw new Exception("Unable to determine path to load assemblies from") : assemblyLocation;
    }

    private static Type? GetAssertionsFactoryType(
        AssertionFrameworks assertionFramework,
        int majorVersion,
        string? assemblyLocation = null)
    {
        assemblyLocation ??= GetInMemoryExtensionsAssemblyLocation();

        var versionedLocation = Path.Combine(
            assemblyLocation,
            $"Serilog.Sinks.InMemory.{assertionFramework:G}{majorVersion}.dll");

        if (!File.Exists(versionedLocation))
        {
            throw new InvalidOperationException(
                $"Detected {assertionFramework:G} version {majorVersion} but the assertions adapter wasn't found on disk");
        }

        var versionedAssembly = Assembly.LoadFile(versionedLocation);

        return versionedAssembly.GetTypes()
            .SingleOrDefault(t => t.Name == "InMemorySinkAssertionsFactoryImpl");
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
                (assembly.GetName().Name.Equals("FluentAssertions") || assembly.GetName().Name.Equals("AwesomeAssertions")) &&
                assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                    .Any(metadata => metadata.Value.Contains("AwesomeAssertions", StringComparison.OrdinalIgnoreCase)));

        return awesomeAssertionsAssembly != null;
    }

    private static bool IsShouldlyAlreadyLoadedIntoDomain(
        [NotNullWhen(true)] out Assembly? shouldlyAssembly)
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

            if (!metadataAttributes.Any() ||
                metadataAttributes.Any(metadata => metadata.Value.Contains("FluentAssertions", StringComparison.OrdinalIgnoreCase)))
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
        var assemblyPath = Path.Combine(assemblyLocation, "AwesomeAssertions.dll");

        if (File.Exists(assemblyPath))
        {
            assembly = Assembly.LoadFile(assemblyPath);

            if (assembly.GetCustomAttributes<AssemblyProductAttribute>()
                .Any(product =>
                    product.Product.Equals("AwesomeAssertions", StringComparison.CurrentCultureIgnoreCase)))
            {
                return true;
            }

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
}