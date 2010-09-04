//////////////////////////////////////////////////////////////////////
// Algorithmia is (c) 2009 Solutions Design. All rights reserved.
// http://www.sd.nl
//////////////////////////////////////////////////////////////////////
// COPYRIGHTS:
// Copyright (c) 2009 Solutions Design. All rights reserved.
// 
// The Algorithmia library sourcecode and its accompanying tools, tests and support code
// are released under the following license: (BSD2)
// ----------------------------------------------------------------------
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met: 
//
// 1) Redistributions of source code must retain the above copyright notice, this list of 
//    conditions and the following disclaimer. 
// 2) Redistributions in binary form must reproduce the above copyright notice, this list of 
//    conditions and the following disclaimer in the documentation and/or other materials 
//    provided with the distribution. 
// 
// THIS SOFTWARE IS PROVIDED BY SOLUTIONS DESIGN ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL SOLUTIONS DESIGN OR CONTRIBUTORS BE LIABLE FOR 
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
// BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
//
// The views and conclusions contained in the software and documentation are those of the authors 
// and should not be interpreted as representing official policies, either expressed or implied, 
// of Solutions Design. 
//
//////////////////////////////////////////////////////////////////////
// Contributers to the code:
//		- Frans  Bouma [FB]
//////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xpand.Utils.GeneralDataStructures
{
    /// <summary>
    /// Extension to the normal Dictionary. This class can store more than one value for every key. It keeps a HashSet for every Key value.
    /// Calling Add with the same Key and multiple values will store each value under the same Key in the Dictionary. Obtaining the values
    /// for a Key will return the HashSet with the Values of the Key. 
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class MultiValueDictionary<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>, ILookup<TKey, TValue>
    {
        #region ILookup<TKey,TValue> Members
        /// <summary>
        /// Determines whether a specified key exists in the <see cref="T:System.Linq.ILookup`2"/>.
        /// </summary>
        /// <param name="key">The key to search for in the <see cref="T:System.Linq.ILookup`2"/>.</param>
        /// <returns>
        /// true if <paramref name="key"/> is in the <see cref="T:System.Linq.ILookup`2"/>; otherwise, false.
        /// </returns>
        bool ILookup<TKey, TValue>.Contains(TKey key)
        {
            return ContainsKey(key);
        }


        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of key/value pairs contained in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </returns>
        int ILookup<TKey, TValue>.Count
        {
            get { return Count; }
        }


        /// <summary>
        /// Gets the <see cref="System.Collections.Generic.IEnumerable&lt;TValue&gt;"/> with the specified key.
        /// </summary>
        /// <value></value>
        IEnumerable<TValue> ILookup<TKey, TValue>.this[TKey key]
        {
            get { return GetValues(key, true); }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<IGrouping<TKey, TValue>> IEnumerable<IGrouping<TKey, TValue>>.GetEnumerator()
        {
            foreach (var pair in this)
            {
                yield return new Grouping<TKey, TValue>(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        /// <summary>
        /// Adds the specified value under the specified key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            

            HashSet<TValue> container;
            if (!TryGetValue(key, out container))
            {
                container = new HashSet<TValue>();
                Add(key, container);
            }
            container.Add(value);
        }


        /// <summary>
        /// Adds the range of values under the key specified.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        public void AddRange(TKey key, IEnumerable<TValue> values)
        {
            if (values == null)
            {
                return;
            }

            foreach (TValue value in values)
            {
                Add(key, value);
            }
        }


        /// <summary>
        /// Determines whether this dictionary contains the specified value for the specified key 
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if the value is stored for the specified key in this dictionary, false otherwise</returns>
        public bool ContainsValue(TKey key, TValue value)
        {
            
            bool toReturn = false;
            HashSet<TValue> values;
            if (TryGetValue(key, out values))
            {
                toReturn = values.Contains(value);
            }
            return toReturn;
        }


        /// <summary>
        /// Removes the specified value for the specified key. It will leave the key in the dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Remove(TKey key, TValue value)
        {
            

            HashSet<TValue> container;
            if (TryGetValue(key, out container))
            {
                container.Remove(value);
                if (container.Count <= 0)
                {
                    Remove(key);
                }
            }
        }


        /// <summary>
        /// Merges the specified multivaluedictionary into this instance.
        /// </summary>
        /// <param name="toMergeWith">To merge with.</param>
        public void Merge(MultiValueDictionary<TKey, TValue> toMergeWith)
        {
            if (toMergeWith == null)
            {
                return;
            }

            foreach (var pair in toMergeWith)
            {
                foreach (TValue value in pair.Value)
                {
                    Add(pair.Key, value);
                }
            }
        }


        /// <summary>
        /// Gets the values for the key specified. This method is useful if you want to avoid an exception for key value retrieval and you can't use TryGetValue
        /// (e.g. in lambdas)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="returnEmptySet">if set to true and the key isn't found, an empty hashset is returned, otherwise, if the key isn't found, null is returned</param>
        /// <returns>
        /// This method will return null (or an empty set if returnEmptySet is true) if the key wasn't found, or
        /// the values if key was found.
        /// </returns>
        public HashSet<TValue> GetValues(TKey key, bool returnEmptySet)
        {
            HashSet<TValue> toReturn;
            if (!TryGetValue(key, out toReturn) && returnEmptySet)
            {
                toReturn = new HashSet<TValue>();
            }
            return toReturn;
        }
    }
}