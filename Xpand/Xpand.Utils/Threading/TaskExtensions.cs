using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Xpand.Utils.Threading{
    public static class TaskExtensions{
        internal static void MarshalTaskResults<TResult>(Task source, TaskCompletionSource<TResult> proxy){
            switch (source.Status){
                case TaskStatus.RanToCompletion:{
                    var task = source as Task<TResult>;
                    proxy.TrySetResult((task == null) ? default(TResult) : task.Result);
                    break;
                }
                case TaskStatus.Canceled:
                    proxy.TrySetCanceled();
                    break;

                case TaskStatus.Faulted:{
                    AggregateException exception = source.Exception;
                    Debug.Assert(exception != null, "aggregateException != null");
                    proxy.TrySetException(exception);
                    break;
                }
            }
        }

        public static Task TimeoutAfter(this Task task, int millisecondsTimeout){
            if (task.IsCompleted || (millisecondsTimeout == -1)){
                return task;
            }
            var tcs = new TaskCompletionSource<VoidTypeStruct>();
            if (millisecondsTimeout == 0){
                tcs.SetException(new TimeoutException());
                return tcs.Task;
            }
            var timer =
                new Timer(
                    delegate(object state){
                        ((TaskCompletionSource<VoidTypeStruct>) state).TrySetException(new TimeoutException());
                    }, tcs,
                    millisecondsTimeout, -1);
            task.ContinueWith(delegate(Task antecedent){
                timer.Dispose();
                MarshalTaskResults(antecedent, tcs);
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return tcs.Task;
        }

        public static Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, int millisecondsTimeout){
            if (task.IsCompleted || (millisecondsTimeout == -1)){
                return task;
            }
            var tcs = new TaskCompletionSource<TResult>();
            if (millisecondsTimeout == 0){
                tcs.SetException(new TimeoutException());
                return tcs.Task;
            }
            var timer =
                new Timer(
                    delegate(object state){
                        ((TaskCompletionSource<TResult>) state).TrySetException(new TimeoutException());
                    }, tcs,
                    millisecondsTimeout, -1);
            task.ContinueWith(delegate(Task<TResult> antecedent){
                timer.Dispose();
                MarshalTaskResults(antecedent, tcs);
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return tcs.Task;
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        internal struct VoidTypeStruct{
        }
    }
}