using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.Utils.Linq {
    public static class Extensions {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }
    }
}
