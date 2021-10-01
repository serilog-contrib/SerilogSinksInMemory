# Changelog

## Serilog.Sinks.InMemory.Assertions: 0.9.0

This release introduces support for FluentAssertions 6.x and maintains backwards compatibility with FluentAssertions 5.x releases.
A test project has been added to verify this compatibility, see Serilog.Sinks.InMemory.Assertions.Tests.Unit.FluentAssertions6

This release also:

- Upgrades xUnit to 2.4.1 and xUnit VS runner to 2.4.3
- Upgrades Serilog to 2.10.0
- Formats the `LogEventLevel` values so that they are always presented in assertion messages as `"Information"`. This is to ensure the assertions show consistent behaviour when using FluentAssertions 5 or 6
- Adds netcoreapp3.1 as a test target
- Removes netcoreapp2.0 as a test target as it's no longer supported