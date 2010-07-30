using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace eXpand.Utils.Linq
{
    public static class RecursiveLinq
    {
        private delegate Func<A, R> Recursive<A, R>(Recursive<A, R> r);
        public static Func<A, R> Y<A, R>(Func<Func<A, R>, Func<A, R>> f)
        {
            Recursive<A, R> rec = r => a => f(r(r))(a);
            return rec(rec);
        }

        public static IEnumerable<T> GetRecursively<T>(this IEnumerable collection, Func<T, IEnumerable> selector,
                                                       Func<T, bool> predicate) {
            foreach (T item in collection.OfType<T>()) {
                if (!predicate(item)) continue;

                yield return item;

                IEnumerable<T> children = selector(item).GetRecursively(selector, predicate);
                foreach (T child in children) {
                    yield return child;
                }
            }
        }
    }
}
