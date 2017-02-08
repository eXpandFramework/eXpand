using System;
using System.Threading;

namespace Xpand.Utils.Threading{
    /// <summary>Defines a provider for progress updates.</summary>
    /// <typeparam name="T">The type of progress update value.</typeparam>
    public interface IProgress<in T> {
        /// <summary>Reports a progress update.</summary>
        /// <param name="value">The value of the updated progress.</param>
        void Report(T value);
    }

    public class Progress<T> : IProgress<T> {
        readonly SynchronizationContext _context;
        public Progress() {
            _context = SynchronizationContext.Current
                       ?? new SynchronizationContext();
        }

        public Progress(Action<T> action)
            : this() {
            ProgressReported += action;
        }

        public event Action<T> ProgressReported;

        void IProgress<T>.Report(T data) {
            var action = ProgressReported;
            if (action != null) {
                _context.Post(arg => action((T)arg), data);
            }
        }
    }
}