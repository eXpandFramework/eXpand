using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Xpand.Utils.Threading{
    public static class TaskExtensions{
        /// <summary>
        ///     Starts the periodic task.
        /// </summary>
        /// <param name="taskFactory"></param>
        /// <param name="action">The action.</param>
        /// <param name="taskScheduler"></param>
        /// <param name="interval">The interval in milliseconds.</param>
        /// <param name="delay">The delay in milliseconds, i.e. how long it waits to kick off the timer.</param>
        /// <param name="duration">
        ///     The duration.
        ///     <example>If the duration is set to 10 seconds, the maximum time this task is allowed to run is 10 seconds.</example>
        /// </param>
        /// <param name="maxIterations">The max iterations.</param>
        /// <param name="synchronous">
        ///     if set to <c>true</c> executes each period in a blocking fashion and each periodic execution of the task
        ///     is included in the total duration of the Task.
        /// </param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <param name="periodicTaskCreationOptions">
        ///     <see cref="Task" /> used to create the task for executing the
        ///     <see cref="Action" />.
        /// </param>
        /// <returns>A <see cref="Action" /></returns>
        /// <remarks>
        ///     Exceptions that occur in the <paramref name="action" /> need to be handled in the action itself. These exceptions
        ///     will not be
        ///     bubbled up to the periodic task.
        /// </remarks>
        public static Task StartNewPeriodic(this TaskFactory taskFactory, Action action, TaskScheduler taskScheduler=null,
            int interval = Timeout.Infinite,
            int delay = 0,
            int duration = Timeout.Infinite,
            int maxIterations = -1,
            bool synchronous = false,
            CancellationToken cancelToken = new CancellationToken(),
            TaskCreationOptions periodicTaskCreationOptions = TaskCreationOptions.None){

            taskScheduler = taskScheduler ?? TaskScheduler.Default;
            var stopWatch = new Stopwatch();

            void WrapperAction(){
                CheckIfCancelled(cancelToken);
                action();
            }

            void MainAction(){
                MainPeriodicTaskAction(interval, delay, duration, maxIterations, cancelToken, stopWatch, synchronous, WrapperAction, periodicTaskCreationOptions, taskScheduler);
            }

            return Task.Factory.StartNew(MainAction, cancelToken, TaskCreationOptions.LongRunning, taskScheduler);
        }

        /// <summary>
        ///     Mains the periodic task action.
        /// </summary>
        /// <param name="intervalInMilliseconds">The interval in milliseconds.</param>
        /// <param name="delayInMilliseconds">The delay in milliseconds.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="maxIterations">The max iterations.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <param name="stopWatch">The stop watch.</param>
        /// <param name="synchronous">
        ///     if set to <c>true</c> executes each period in a blocking fashion and each periodic execution of the task
        ///     is included in the total duration of the Task.
        /// </param>
        /// <param name="wrapperAction">The wrapper action.</param>
        /// <param name="periodicTaskCreationOptions">
        ///     <see cref="TaskCreationOptions" /> used to create a sub task for executing
        ///     the <see cref="Action" />.
        /// </param>
        /// <param name="taskScheduler"></param>
        private static void MainPeriodicTaskAction(int intervalInMilliseconds, int delayInMilliseconds, int duration, int maxIterations, CancellationToken cancelToken, Stopwatch stopWatch, bool synchronous, Action wrapperAction, TaskCreationOptions periodicTaskCreationOptions, TaskScheduler taskScheduler) {
            var subTaskCreationOptions = TaskCreationOptions.AttachedToParent | periodicTaskCreationOptions;

            CheckIfCancelled(cancelToken);

            if (delayInMilliseconds > 0) Thread.Sleep(delayInMilliseconds);

            if (maxIterations == 0) return;

            var iteration = 0;

            ////////////////////////////////////////////////////////////////////////////
            // using a ManualResetEventSlim as it is more efficient in small intervals.
            // In the case where longer intervals are used, it will automatically use 
            // a standard WaitHandle....
            // see http://msdn.microsoft.com/en-us/library/vstudio/5hbefs30(v=vs.100).aspx
            using (var periodResetEvent = new ManualResetEventSlim(false)) {
                ////////////////////////////////////////////////////////////
                // Main periodic logic. Basically loop through this block
                // executing the action
                while (true) {
                    CheckIfCancelled(cancelToken);

                    var subTask = Task.Factory.StartNew(wrapperAction, cancelToken, subTaskCreationOptions,taskScheduler);

                    if (synchronous) {
                        stopWatch.Start();
                        try {
                            subTask.Wait(cancelToken);
                        }
                        catch {
                            /* do not let an errant subtask to kill the periodic task...*/
                        }
                        stopWatch.Stop();
                    }

                    // use the same Timeout setting as the System.Threading.Timer, infinite timeout will execute only one iteration.
                    if (intervalInMilliseconds == Timeout.Infinite) break;

                    iteration++;

                    if ((maxIterations > 0) && (iteration >= maxIterations)) break;

                    try {
                        stopWatch.Start();
                        periodResetEvent.Wait(intervalInMilliseconds, cancelToken);
                        stopWatch.Stop();
                    }
                    finally {
                        periodResetEvent.Reset();
                    }

                    CheckIfCancelled(cancelToken);

                    if ((duration > 0) && (stopWatch.ElapsedMilliseconds >= duration)) break;
                }
            }
        }

        /// <summary>
        ///     Checks if cancelled.
        /// </summary>
        private static void CheckIfCancelled(CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
        }

        internal static void MarshalTaskResults<TResult>(Task source, TaskCompletionSource<TResult> proxy){
            switch (source.Status){
                case TaskStatus.RanToCompletion:{
                    var task = source as Task<TResult>;
                    proxy.TrySetResult((task == null) ? default : task.Result);
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

        public static CancellationTokenSource Execute(this int timeout) {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            bool started = false;

            Task.Factory.StartNew(() => {
                started = true;
                while (!cancellationToken.IsCancellationRequested) {
                    Thread.Sleep(200);
                }
                cancellationToken.ThrowIfCancellationRequested();
            }, cancellationToken).TimeoutAfter(timeout);

            while (!started) {
                Thread.Sleep(100);
            }
            return cancellationTokenSource;
        }

        public static bool WaitToCompleteOrTimeOut(this Task task){
            try{
                Task.WaitAll(task);
                return true;
            }
            catch (AggregateException e){
                if (!e.InnerExceptions.All(exception => exception is TimeoutException))
                    throw;
                return false;
            }
        }

        public static Task TimeoutAfter(this Task task, int millisecondsTimeout){
            if (task.IsCompleted || (millisecondsTimeout == -1)){
                return task;
            }
            var taskCompletionSource = new TaskCompletionSource<VoidTypeStruct>();
            if (millisecondsTimeout == 0){
                taskCompletionSource.SetException(new TimeoutException());
                return taskCompletionSource.Task;
            }
            var timer =new Timer(state =>{
                ((TaskCompletionSource<VoidTypeStruct>)state).TrySetException(new TimeoutException());
            }, taskCompletionSource,(long) millisecondsTimeout, -1);
            task.ContinueWith(delegate(Task antecedent){
                timer.Dispose();
                MarshalTaskResults(antecedent, taskCompletionSource);
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return taskCompletionSource.Task;
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
                    state => ((TaskCompletionSource<TResult>) state).TrySetException(new TimeoutException()), tcs,
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