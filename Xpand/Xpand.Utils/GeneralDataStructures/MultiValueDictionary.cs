using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Xpand.Utils.GeneralDataStructures {
    public class ConcurrentHashSet<T> : IDisposable, IEnumerable<T> {
        private readonly HashSet<T> _hashSet = new HashSet<T>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public int Count {
            get {
                _lock.EnterReadLock();

                try {
                    return _hashSet.Count;
                }
                finally {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerator<T> GetEnumerator() {
            _lock.EnterWriteLock();

            try {
                return _hashSet.GetEnumerator();
            }
            finally {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public bool TryAdd(T item) {
            _lock.EnterWriteLock();

            try {
                return _hashSet.Add(item);
            }
            finally {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public void Clear() {
            _lock.EnterWriteLock();

            try {
                _hashSet.Clear();
            }
            finally {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public bool Contains(T item) {
            _lock.EnterReadLock();

            try {
                return _hashSet.Contains(item);
            }
            finally {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }

        public bool TryRemove(T item) {
            _lock.EnterWriteLock();

            try {
                return _hashSet.Remove(item);
            }
            finally {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public T FirstOrDefault(Func<T, bool> predicate) {
            _lock.EnterReadLock();

            try {
                return _hashSet.FirstOrDefault(predicate);
            }
            finally {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) _lock?.Dispose();
        }
    }

    public class
        MultiValueDictionary<TKey, TValue> : ConcurrentDictionary<TKey, ConcurrentHashSet<TValue>>, ILookup<TKey, TValue> {
        bool ILookup<TKey, TValue>.Contains(TKey key) {
            return ContainsKey(key);
        }

        int ILookup<TKey, TValue>.Count => Count;

        IEnumerable<TValue> ILookup<TKey, TValue>.this[TKey key] => GetValues(key, true);

        IEnumerator<IGrouping<TKey, TValue>> IEnumerable<IGrouping<TKey, TValue>>.GetEnumerator() {
            foreach (var pair in this) yield return new Grouping<TKey, TValue>(pair.Key, pair.Value);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(TKey key, TValue value) {
            if (!TryGetValue(key, out var container)) {
                container = new ConcurrentHashSet<TValue>();
                TryAdd(key, container);
            }

            container.TryAdd(value);
        }

        public void AddRange(TKey key, IEnumerable<TValue> values) {
            if (values == null) return;

            foreach (var value in values) Add(key, value);
        }

        public bool ContainsValue(TKey key, TValue value) {
            var toReturn = false;
            if (TryGetValue(key, out var values)) toReturn = values.Contains(value);
            return toReturn;
        }

        public void Remove(TKey key, TValue value) {
            if (TryGetValue(key, out var container)) {
                container.TryRemove(value);
                if (container.Count <= 0) TryRemove(key, out container);
            }
        }

        public void Merge(MultiValueDictionary<TKey, TValue> toMergeWith) {
            if (toMergeWith == null) return;

            foreach (var pair in toMergeWith)
            foreach (var value in pair.Value)
                Add(pair.Key, value);
        }

        public ConcurrentHashSet<TValue> GetValues(TKey key, bool returnEmptySet) {
            if (!TryGetValue(key, out var toReturn) && returnEmptySet) toReturn = new ConcurrentHashSet<TValue>();
            return toReturn;
        }
    }
}