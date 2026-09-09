using System;
using System.Collections;
using System.Linq;

namespace Serilog.Sinks.InMemory.Shouldly4
{
    static class PropertyValueEquality
    {
        /// <summary>
        /// Compares two property values for equality, handling both scalar values
        /// and enumerable sequences with order-sensitive, element-by-element comparison.
        /// </summary>
        /// <remarks>
        /// This performs a shallow comparison - nested collections are compared by reference,
        /// not recursively. String values are handled by object.Equals() before enumerable
        /// comparison, so strings work correctly despite being IEnumerable.
        /// </remarks>
        public static new bool Equals(object x, object y)
            => object.Equals(x, y)
                || (x is IEnumerable xe
                    && y is IEnumerable ye
                    && xe.Cast<object>().SequenceEqual(ye.Cast<object>()));
    }
}
