#if NET6_0

using System;
using PublicApiGenerator;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.InMemory.Tests.Unit;

/// <summary>Tests for checking changes to the public API.</summary>
public class ApiApprovalTests
{
    /// <summary>Check for changes to the public APIs.</summary>
    /// <param name="type">The type used as a marker for the assembly whose public API change you want to check.</param>
    [Theory]
    [InlineData(typeof(InMemorySinkExtensions))]
    public void PublicApi_Should_Not_Change_Unintentionally(Type type)
    {
        string publicApi = type.Assembly.GeneratePublicApi(new()
        {
            IncludeAssemblyAttributes = false,
            AllowNamespacePrefixes = new[] { "System", "Microsoft.Extensions.DependencyInjection" },
            ExcludeAttributes = new[] { "System.Diagnostics.DebuggerDisplayAttribute" },
        });
        publicApi.ShouldMatchApproved(options => options.NoDiff().WithFilenameGenerator((testMethodInfo, discriminator, fileType, fileExtension) => $"{type.Assembly.GetName().Name!}.{fileType}.{fileExtension}"));
    }
}

#endif
