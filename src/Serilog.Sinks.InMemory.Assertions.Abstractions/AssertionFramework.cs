#nullable enable
using System;

namespace Serilog.Sinks.InMemory.Assertions;

public readonly struct AssertionFramework
{
    public AssertionFramework(AssertionFrameworks framework, Version version)
    {
        Framework = framework;
        Version = version;
    }

    public AssertionFrameworks Framework { get; }

    public Version Version { get; }
}