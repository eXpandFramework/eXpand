using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xpand.CodeRush.Plugins.Extensions {
    public static class RecursiveLinq {

        public static Task StartNewNow(this TaskFactory taskFactory, Action action){
            return taskFactory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }

        public static IEnumerable<T> GetItems<T>(this IEnumerable collection,
                Func<T, IEnumerable> selector) {
            var stack = new Stack<IEnumerable<T>>();
            stack.Push(collection.OfType<T>());

            while (stack.Count > 0) {
                IEnumerable<T> items = stack.Pop();
                foreach (var item in items) {
                    yield return item;

                    IEnumerable<T> children = selector(item).OfType<T>();
                    stack.Push(children);
                }
            }
        }
    }
}
