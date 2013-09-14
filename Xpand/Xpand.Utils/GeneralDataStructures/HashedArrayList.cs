using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xpand.Utils.GeneralDataStructures {
    
    public class HashedArrayList : ArrayList {
        private readonly HashSet<object> hashSet = new HashSet<object>();

        /// <summary>
        /// Overridden
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool Contains(object item) {
            return hashSet.Contains(item);
        }
        /// <summary>
        ///  Overridden
        /// </summary>
        public override void Clear() {
            hashSet.Clear();
            base.Clear();
        }

        /// <summary>
        /// Overridden
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override int Add(object value) {
            if (!hashSet.Add(value)) {
                throw new Exception("It is not expected that a bo is added multiple times");
            }
            return base.Add(value);
        }

        /// <summary>
        /// Overridden
        /// </summary>
        /// <param name="c"></param>
        public override void AddRange(ICollection c) {
            if (c == null) {
                throw new ArgumentNullException("c");
            }

            if (c.Cast<object>().Any(o => !hashSet.Add(o))) {
                throw new Exception("It is not expected that a bo is added multiple times");
            }
            base.AddRange(c);
        }

        /// <summary>
        /// Overridden
        /// </summary>
        /// <returns></returns>
        public override object Clone() {
            throw new Exception("Call is not expected");
        }

        /// <summary>
        /// Overridden
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public override void Insert(int index, object value) {
            if (!hashSet.Add(value)) {
                throw new Exception("It is not expected that a bo is added multiple times");
            }
            base.Insert(index, value);
        }

        /// <summary>
        /// Overridden
        /// </summary>
        /// <param name="index"></param>
        /// <param name="c"></param>
        public override void InsertRange(int index, ICollection c) {
            if (c == null) {
                throw new ArgumentNullException("c");
            }
            if (c.Cast<object>().Any(o => !hashSet.Add(o))) {
                throw new Exception("It is not expected that a bo is added multiple times");
            }
            base.InsertRange(index, c);
        }

        /// <summary>
        /// Overridden
        /// </summary>
        /// <param name="obj"></param>
        public override void Remove(object obj) {
            hashSet.Remove(obj);
            base.Remove(obj);
        }

        /// <summary>
        /// Overridden
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt(int index) {
            hashSet.Remove(this[index]);
            base.Remove(index);
        }

        /// <summary>
        /// Overridden
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public override void RemoveRange(int index, int count) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overridden
        /// </summary>
        /// <param name="index"></param>
        /// <param name="c"></param>
        public override void SetRange(int index, ICollection c) {
            throw new NotImplementedException();
        }
    }
}
