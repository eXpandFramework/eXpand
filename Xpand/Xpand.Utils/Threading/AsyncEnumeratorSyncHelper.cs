using System;
using System.Threading;
using Wintellect.Threading.AsyncProgModel;

namespace Xpand.Utils.Threading
{
    public class AsyncEnumeratorSyncHelper
    {
        private AsyncEnumeratorSyncHelper()
        {
        }

        public static AsyncResult<T> BeginHelper<T>(AsyncCallback callback, object state, Func<T> method)
        {
            var ar = new AsyncResult<T>(callback, state);

            Action<object> work = asyncResult => ExecuteHelper(method, (AsyncResult<T>)asyncResult);
            ThreadPool.QueueUserWorkItem(new WaitCallback(work), ar);

            return ar;
        }

        public static AsyncResult BeginHelper(AsyncCallback callback, object state, Action method)
        {
            // just dummy object
            return BeginHelper<object>(callback, state, () =>
            {
                method();
                return null;
            });
        }

        public static T EndHelper<T>(IAsyncResult asyncResult)
        {
            var ar = (AsyncResult<T>)asyncResult;

            return ar.EndInvoke();
        }

        public static void EndHelper(IAsyncResult asyncResult)
        {
            // just dummy object
            EndHelper<object>(asyncResult);
        }

        private static void ExecuteHelper<T>(Func<T> method, AsyncResult<T> asyncResult)
        {
            try
            {
                T result = method();
                asyncResult.SetAsCompleted(result, false);
            }
            catch (Exception ex)
            {
                asyncResult.SetAsCompleted(ex, false);
            }
        }
    }
}
