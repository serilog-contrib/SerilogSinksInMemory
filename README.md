# Serilog.Sinks.InMemory
In-memory sink for Serilog to use for testing with [FluentAssertions](https://fluentassertions.com/) support for easy to write assertions.

## Build status
[![Build status](https://ci.appveyor.com/api/projects/status/aq0g2f247q5proix?svg=true)](https://ci.appveyor.com/project/sandermvanvliet/serilogsinksinmemory)
[![NuGet Serilog.Sinks.InMemory](https://buildstats.info/nuget/Serilog.Sinks.InMemory)](https://www.nuget.org/packages/Serilog.Sinks.InMemory/)
[![NuGet Serilog.Sinks.InMemory.Assertions](https://buildstats.info/nuget/Serilog.Sinks.InMemory.Assertions)](https://www.nuget.org/packages/Serilog.Sinks.InMemory.Assertions/)

## Usage

To just use the sink, add the `Serilog.Sinks.InMemory` NuGet package:

`dotnet` CLI:
```bash
dotnet add package Serilog.Sinks.InMemory
```

PowerShell:
```PowerShell
Install-Package Serilog.Sinks.InMemory
```

But it's better with assertions so you'll also want to add the `Serilog.Sinks.InMemory.Assertions` NuGet package:

`dotnet` CLI:
```bash
dotnet add package Serilog.Sinks.InMemory.Assertions
```

PowerShell:
```PowerShell
Install-Package Serilog.Sinks.InMemory.Assertions
```

## Example
Let's say you have a class with method implementing some complicated business logic:

```csharp
public class ComplicatedBusinessLogic
{
    private readonly ILogger _logger;

    public ComplicatedBusinessLogic(ILogger logger)
    {
        _logger = logger;
    }

    public string FirstTenCharacters(string input)
    {
        return input.Substring(0, 10);
    }
}
```

A request came in to log a message with the number of characters in the input. So to test that you can create a mock of `ILogger` and assert the method to log was called, however mock setups quickly become very messy (true: this is my opinion!) and assertions on mocks have the same problem when you start verifying values of arguments.

So instead let's use Serilog and a dedicated sink for testing:

```csharp
public class WhenExecutingBusinessLogic
{
    public void GivenInputOfFiveCharacters_MessageIsLogged()
    {
        var logger = new LoggerConfiguration()
            .WriteTo.InMemory()
            .CreateLogger();

        var logic = new ComplicatedBusinessLogic(logger);

        logic.FirstTenCharacters("12345");

        // Use the static Instance property to access the in-memory sink
        InMemorySink.Instance
            .Should()
            .HaveMessage("Input is {count} characters long");
    }
}
```

The test will now fail with `Expected a message to be logged with template \"Input is {count} characters long\" but didn't find any`

Now change the implementation to:

```csharp
public string FirstTenCharacters(string input)
{
    _logger.Information("Input is {count} characters long", input.Length);

    return input.Substring(0, 10);
}
```

Run the test again and it now passes. But how do we ensure this message is only logged once?

To do that, create a new test like so:

```csharp
public void GivenInputOfFiveCharacters_MessageIsLoggedOnce()
{
    /* omitted for brevity */

    InMemorySink.Instance
        .Should()
        .HaveMessage("Input is {count} characters long")
        .Appearing().Once();
}
```

To verify if a message is logged multiple times use `AppearsTimes(int numberOfTimes)`

So now you'll want to verify that the property `count` has the expected value. This builds upon the previous test:

```csharp
public void GivenInputOfFiveCharacters_CountPropertyValueIsFive()
{
    /* omitted for brevity */

    InMemorySink.Instance
        .Should()
        .HaveMessage("Input is {count} characters long")
        .Appearing().Once();
        .WithProperty("count")
        .WithValue(5);
}
```

### Asserting a message appears more than once

Let's say you have a log message in a loop and you want to verify that:

```csharp
public void GivenLoopWithFiveItems_MessageIsLoggedFiveTimes()
{
    /* omitted for brevity */

    InMemorySink.Instance
        .Should()
        .HaveMessage("Input is {count} characters long")
        .Appearing().Times(5);
}
```

### Asserting a message has a certain level

Apart from a message being logged, you'll also want to verify it is of the right level. You can do that using the `WithLevel()` assertion:

```csharp
public void GivenLoopWithFiveItems_MessageIsLoggedFiveTimes()
{
    /* omitted for brevity */

    InMemorySink.Instance
        .Should()
        .HaveMessage("Input is {count} characters long")
        .Appearing().Once()
        .WithLevel(LogEventLevel.Information);
}
```

This also works for multiple messages:

```csharp
public void GivenLoopWithFiveItems_MessageIsLoggedFiveTimes()
{
    logger.Warning("Test message");
    logger.Warning("Test message");
    logger.Warning("Test message");

    InMemorySink.Instance
        .Should()
        .HaveMessage("Test message")
        .Appearing().Times(3)
        .WithLevel(LogEventLevel.Information);
}
```

This will fail with a message: `Expected instances of log message "Hello, world!" to have level Information, but found 3 with level Warning`

### Asserting messages with a pattern

Instead of matching on the exact message you can also match on a certain pattern using the `Containing()` assertion:

```csharp
InMemorySink.Instance
   .Should()
   .HaveMessage()
   .Containing("some pattern")
   .Appearing().Once();
```

which matches on log messages:

- `this is some pattern`
- `some pattern in a message`
- `this is some pattern in a message`

### Asserting messages have been logged at all (or not!)

When you want to assert that a message has been logged but don't care about what message you can do that with `HaveMessage` and `Appearing`:

```csharp
InMemorySink.Instance
                .Should()
                .HaveMessage()
                .Appearing().Times(3); // Expect three messages to be logged
```

and of course the inverse is also possible when expecting no messages to be logged:

```csharp
InMemorySink.Instance
                .Should()
                .NotHaveMessage();
```

or that a specific message is not be logged

```csharp
InMemorySink.Instance
                .Should()
                .NotHaveMessage("a specific message");
```

### Asserting properties on messages

When you want to assert that a message has a property you can do that using the `WithProperty` assertion:

```csharp
InMemorySink.Instance
    .Should()
    .HaveMessage("Message with {Property}")
    .Appearing().Once()
    .WithProperty("Property");
```

To then assert that it has a certain value you would use `WithValue`:

```csharp
InMemorySink.Instance
    .Should()
    .HaveMessage("Message with {Property}")
    .Appearing().Once()
    .WithProperty("Property")
    .WithValue("property value");
```

Asserting that a message has multiple properties can be accomplished using the `And` constraint:

```csharp
InMemorySink.Instance
    .Should()
    .HaveMessage("Message with {Property1} and {Property2}")
    .Appearing().Once()
    .WithProperty("Property1")
    .WithValue("value 1")
    .And
    .WithProperty("Property2")
    .WithValue("value 2");
```

When you have a log message that appears a number of times and you want to assert that the value of the log property has the expected values you can do that using the `WithValues` assertion:

```csharp
InMemorySink.Instance
    .Should()
    .HaveMessage("Message with {Property1} and {Property2}")
    .Appearing().Times(3)
    .WithProperty("Property1")
    .WithValue("value 1", "value 2", "value 3")
```

> **Note:** `WithValue` takes an array of values.

Sometimes you might want to use assertions like `BeLessThanOrEqual()` or `HaveLength()` and in those cases `WithValue` is not very helpful.
Instead you can use `WhichValue<T>()`  to access the value of the log property:

```csharp
InMemorySink.Instance
    .Should()
    .HaveMessage()
    .Appearing().Once()
    .WithProperty("PropertyOne")
    .WhichValue<string>()
    .Should()
    .HaveLength(3);
```

If the type of the value of the log property does not match the generic type parameter the `WhichValue<T>` method will throw an exception.

> **Note:** This only works for scalar values. When you pass an object as the property value when logging a message Serilog converts that into a string.

## Clearing log events between tests

Depending on your test framework and test setup you may want to ensure that the log events captured by the `InMemorySink` are cleared so tests
are not interfering with eachother. To enable this, the `InMemorySink` implements the [`IDisposable`](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netstandard-2.0) interface.
When `Dispose()` is called the `LogEvents` collection is cleared.

It will depend on the test framework or your test if you need this feature. With xUnit this feature is not necessary as it isolates each test in its own instance of the test class which means that they all
have their own instance of the `InMemorySink`. MSTest however has a different approach and there you may want to use this feature as follows:

```csharp
[TestClass]
public class WhenDemonstratingDisposableFeature
{
    private Logger _logger;

    [TestInitialize]
    public void Initialize()
    {
        _logger?.Dispose();

        _logger = new LoggerConfiguration()
            .WriteTo.InMemory()
            .CreateLogger();
    }

    [TestMethod]
    public void GivenAFoo_BarIsBlah()
    {
        _logger.Information("Foo");

        InMemorySink.Instance
            .Should()
            .HaveMessage("Foo");
    }

    [TestMethod]
    public void GivenABar_BazIsQuux()
    {
        _logger.Information("Bar");

        InMemorySink.Instance
            .Should()
            .HaveMessage("Bar");
    }
}
```
this approach ensures that the `GivenABar_BazIsQuux` does not see any messages logged in a previous test.

## Creating a logger

Loggers are created using a LoggerConfiguration object.
A default initiation would be as follows:

```csharp
var logger = new LoggerConfiguration()
    .WriteTo.InMemory()
    .CreateLogger();
```

### Output templates

Text-based sinks use output templates to control formatting. this can be modified through the outputTemplate parameter:

```csharp
var logger = new LoggerConfiguration()
    .WriteTo.InMemory(outputTemplate: "{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
```

The default template, shown in the example above, uses built-in properties like `Timestamp` and `Level`. Refer to the [offcial documentation](https://github.com/serilog/serilog/wiki/Configuration-Basics#output-templates) for further configuration and explanation of these properties.

### Minimum level

In this example only Information level logs and higher will be written to the InMemorySink.

```csharp
var logger = new LoggerConfiguration()
    .WriteTo.InMemory(restrictedToMinimumLevel: Events.LogEventLevel.Information)
    .CreateLogger();

```

**Default Level** - if no MinimumLevel is specified, then Verbose level events and [higher](https://github.com/serilog/serilog/wiki/Configuration-Basics#minimum-level) will be processed.

### Dynamic levels

If an app needs dynamic level switching, the first step is to create an instance of LoggingLevelSwitch when the logger is being configured:

```csharp
var levelSwitch = new LoggingLevelSwitch();
```

This object defaults the current minimum level to Information, so to make logging more restricted, set its minimum level up-front:

```csharp
levelSwitch.MinimumLevel = LogEventLevel.Warning;
```

When configuring the logger, provide the switch using MinimumLevel.ControlledBy():

```csharp
var log = new LoggerConfiguration()
  .MinimumLevel.ControlledBy(levelSwitch)
  .WriteTo.ColoredConsole()
  .CreateLogger();
```

Now, events written to the logger will be filtered according to the switch’s MinimumLevel property.

To turn the level up or down at runtime, perhaps in response to a command sent over the network, change the property:

```csharp
levelSwitch.MinimumLevel = LogEventLevel.Verbose;
log.Verbose("This will now be logged");
```
