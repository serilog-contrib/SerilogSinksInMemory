global using System;
#if AWESOMEASSERTIONS_9
global using AwesomeAssertions;
#elif SHOULDLY_4
global using Shouldly;
#else
global using FluentAssertions;
#endif
global using Serilog.Events;
global using Xunit;
global using Xunit.Sdk;
global using InMemorySinkAssertions = Serilog.Sinks.InMemory.Assertions.InMemorySinkAssertions;