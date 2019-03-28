using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Xpand.Utils.GeneralDataStructures {
    public class MultiValueDictionary<TKey, TValue> : ConcurrentDictionary<TKey, HashSet<TValue>>, ILookup<TKey, TValue> {
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
                container = new HashSet<TValue>();
                TryAdd(key, container);
            }

            container.Add(value);
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
                container.Remove(value);
                if (container.Count <= 0) {
                    TryRemove(key, out container);
                }
            }
        }

        public void Merge(MultiValueDictionary<TKey, TValue> toMergeWith) {
            if (toMergeWith == null) return;

            foreach (var pair in toMergeWith)
            foreach (var value in pair.Value)
                Add(pair.Key, value);
        }

        public HashSet<TValue> GetValues(TKey key, bool returnEmptySet) {
            if (!TryGetValue(key, out var toReturn) && returnEmptySet) toReturn = new HashSet<TValue>();
            return toReturn;
        }
    }
}