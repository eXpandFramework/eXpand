using System.Collections.Generic;

namespace Xpand.Utils.Threading{
    public class Synchronizer<T> {
        private readonly Dictionary<T, object> _locks;
        private readonly object _myLock;

        public Synchronizer() {
            _locks = new Dictionary<T, object>();
            _myLock = new object();
        }

        public object this[T index] {
            get {
                lock (_myLock) {
                    object result;
                    if (_locks.TryGetValue(index, out result))
                        return result;

                    result = new object();
                    _locks[index] = result;
                    return result;
                }
            }
        }
    }
}