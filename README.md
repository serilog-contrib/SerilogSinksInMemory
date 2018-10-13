# Serilog.Sinks.InMemory
In-memory sink for Serilog to use for testing.

## Build status
[![Build status](https://ci.appveyor.com/api/projects/status/aq0g2f247q5proix?svg=true)](https://ci.appveyor.com/project/sandermvanvliet/serilogsinksinmemory)

## Usage

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
        var inMemorySink = new Serilog.Sinks.InMemory.InMemorySink();

        var logger = new LoggerConfiguration()
            .WriteTo.Sink(inMemorySink)
            .CreateLogger();

        var logic = new ComplicatedBusinessLogic(logger);

        logic.FirstTenCharacters("12345");

        logger
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

    logger
        .Should()
        .HaveMessage("Input is {count} characters long")
        .And
        .AppearsOnce();
}
```

To verify if a message is logged multiple times use `AppearsTimes(int numberOfTimes)`

So now you'll want to verify that the property `count` has the expected value. This builds upon the previous test:

```csharp
public void GivenInputOfFiveCharacters_CountPropertyValueIsFive()
{
    /* omitted for brevity */

    logger
        .Should()
        .HaveMessage("Input is {count} characters long")
        .And
        .AppearsOnce()
        .Which
        .Should()
        .HaveProperty("count")
        .WithValue(5);
}
```