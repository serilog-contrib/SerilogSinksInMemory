# Changelog

## 0.14.0.0

Fix a packaging issue that caused the abstractions not to be part of the package.

## 0.13.0.0

- Support FluentAssertions version 5, 6, 7 and 8
- Support AwesomeAssertions version 8
- Support Shouldly 4.x

## 0.11.0.0

- Add assertions for verifying properties on messages that have a property that contains a destructured object

## 0.10.0.0

With this release the package versions are aligned and every next release will keep them in sync. That should make it easier to figure out which package versions go together, use the same version of both and you'll be fine 👍

## Serilog.Sinks.InMemory 0.7.0

- Introduce `InMemorySinkSnapshot` for testing (see below).
- Change target frameworks for test projects to net462, netcoreapp3.1 and net6.0

## Serilog.Sinks.InMemory.Assertions: 0.9.1

- Use the new snapshot mechanism from `InMemorySink` instead of using reflection to achieve that.

## Serilog.Sinks.InMemory.Assertions: 0.9.0

This release introduces support for FluentAssertions 6.x and maintains backwards compatibility with FluentAssertions 5.x releases.
A test project has been added to verify this compatibility, see Serilog.Sinks.InMemory.Assertions.Tests.Unit.FluentAssertions6

This release also:

- Upgrades xUnit to 2.4.1 and xUnit VS runner to 2.4.3
- Upgrades Serilog to 2.10.0
- Formats the `LogEventLevel` values so that they are always presented in assertion messages as `"Information"`. This is to ensure the assertions show consistent behaviour when using FluentAssertions 5 or 6
- Adds netcoreapp3.1 as a test target
- Removes netcoreapp2.0 as a test target as it's no longer supported