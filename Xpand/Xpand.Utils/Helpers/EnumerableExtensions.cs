using System;
using System.Collections.Generic;
using System.Linq;

namespace Xpand.Utils.Helpers {
    public static class EnumerableExtensions {
        public static bool ContainsExactly<T>(this IEnumerable<T> source, int count){
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            if (source == null)
                throw new ArgumentNullException("source");
            using (var e = source.GetEnumerator()){
                for (int i = 0; i < count; i++){
                    if (!e.MoveNext())
                        return false;
                }
                return !e.MoveNext();
            }
        }

        public static bool ContainsExactly<T>(this IEnumerable<T> source,int count, Func<T, bool> predicate){
            return source.Where(predicate).ContainsExactly(count);
        }

        public static bool ContainsMoreThan<T>(this IEnumerable<T> source,
            int count){
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            if (source == null)
                throw new ArgumentNullException("source");
            return source.Skip(count).Any();
        }

        public static bool ContainsMoreThan<T>(this IEnumerable<T> source,
            int count, Func<T, bool> predicate){
            return source.Where(predicate).ContainsMoreThan(count);

        }
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