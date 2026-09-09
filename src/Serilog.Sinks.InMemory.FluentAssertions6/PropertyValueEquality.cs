using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Serilog.Sinks.InMemory.FluentAssertions6
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

        /// <summary>
        /// Comparer for enumerable property values. Used in Contains() checks.
        /// </summary>
        /// <remarks>
        /// GetHashCode uses RuntimeHelpers.GetHashCode because this comparer is only
        /// used with Enumerable.Contains, which doesn't use hashing internally.
        /// If used with HashSet/Dictionary in the future, replace with structural hash.
        /// </remarks>
        public static readonly IEqualityComparer<object> Comparer = new PropertyValueEqualityComparer();

        sealed class PropertyValueEqualityComparer : IEqualityComparer<object>
        {
            public new bool Equals(object x, object y) => PropertyValueEquality.Equals(x, y);

            public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
