using System;
using System.Threading;
using System.Threading.Tasks;
using Xpand.Utils.Threading;

namespace Xpand.Utils.Helpers {
    public static class Extensions {
        public static CancellationTokenSource Execute(this int timeout, Action afterTimeout) {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            bool started = false;

            Task.Factory.StartNew(() => {
                started = true;
                while (!cancellationToken.IsCancellationRequested) {
                    Thread.Sleep(200);
                }
                cancellationToken.ThrowIfCancellationRequested();
            }, cancellationToken).TimeoutAfter(timeout, afterTimeout);

            while (!started) {
                Thread.Sleep(100);
            }
            return cancellationTokenSource;
        }

    }
}
