﻿using FluentAssertions;

namespace Serilog.Sinks.InMemory.Assertions;

public interface LogEventPropertyValueAssertions
{
    TValue WhichValue<TValue>();
    StructuredValueAssertions HavingADestructuredObject(string because = "", params object[] becauseArgs);
    AndConstraint<LogEventAssertion> WithValue(object value, string because = "", params object[] becauseArgs);
}