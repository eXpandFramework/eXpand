using System;
using System.Threading;
using Wintellect.Threading.AsyncProgModel;

namespace Xpand.Utils.Threading {
    public enum Invoke {
        Sync,
        Async
    }

    public sealed class SingleInstance<T> where T : class {
        readonly Func<T> m_factory;
        readonly Object m_lockObj = new Object();
        readonly Invoke m_options;

        T m_value;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="SingleInstance&lt;T&gt;"/> class.
        /// </summary>
        public SingleInstance()
            : this(() => default(T)) {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="SingleInstance&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="factoryMethod">The factory method.</param>
        public SingleInstance(Func<T> factoryMethod)
            : this(factoryMethod, Invoke.Sync) {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="SingleInstance&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="factory">The factory method.</param>
        /// <param name="options">The options.</param>
        public SingleInstance(Func<T> factory, Invoke options) {
            m_factory = factory;
            m_options = options;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public T Instance {
            get {
                if (m_value != null) {
                    return m_value;
                }

                T temp = Initialize();
                Interlocked.CompareExchange(ref m_value, temp, null);

                return m_value;
            }
        }
        #region Private Methods
        T Initialize() {
            T value = null;

            Boolean lockTaken = false;
            try {
                Monitor.Enter(m_lockObj);
                lockTaken = true;

                if (m_options == Invoke.Async) {
                    var done = new ManualResetEvent(false);
                    BeginInvoke(ar => { value = EndInvoke(ar); done.Set(); }, null);
                    done.WaitOne();
                } else {
                    value = m_factory.Invoke();
                }
            } finally {
                if (lockTaken) {
                    Monitor.Exit(m_lockObj);
                }
            }

            return value;
        }

        void BeginInvoke(AsyncCallback callback, object state) {
            var ar = new AsyncResult<T>(callback, state);

            ThreadPool.QueueUserWorkItem(obj => {
                var asyncResult = (AsyncResult<T>)obj;
                try {
                    asyncResult.SetAsCompleted(m_factory.Invoke(), false);
                } catch (Exception e) {
                    asyncResult.SetAsCompleted(e, false);
                }
            }, ar);

            return;
        }

        T EndInvoke(IAsyncResult asyncResult) {
            var ar = (AsyncResult<T>)asyncResult;
            return ar.EndInvoke();
        }
        #endregion
    }
}