using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xpand.Utils.Helpers{
    public static class Retry {
        public static void Do(
            Action action,
            TimeSpan retryInterval,
            int retryCount = 3) {
            Do<object>(() => {
                action();
                return null;
            }, retryInterval, retryCount);
        }

        public static T Do<T>(
            Func<T> action,
            TimeSpan retryInterval,
            int retryCount = 3) {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++) {
                try {
                    return action();
                }
                catch (Exception ex) {
                    exceptions.Add(ex);
                    var task = Task.Factory.StartNew(() => Thread.Sleep(retryInterval));
                    Task.WaitAll(task);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}