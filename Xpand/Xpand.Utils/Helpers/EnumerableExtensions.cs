using System;
using System.Collections.Generic;
using System.Linq;

namespace Xpand.Utils.Helpers {
    public static class EnumerableExtensions {
        public static string AggregateWith(this IEnumerable<string> instance, string accumulator) {
            return instance.Aggregate("", (current, result) => current + (result + accumulator)).TrimEnd(accumulator.ToCharArray());
        }

        public static void Each<T>(this IEnumerable<T> instance, Action<T> action) {
            foreach (T item in instance) {
                action(item);
            }
        }
    }
}