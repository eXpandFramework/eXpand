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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xpand.Utils.GeneralDataStructures
{
    /// <summary>
    /// Extension methods for classes defined in the GeneralDataStructures namespace
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts an enumerable into a MultiValueDictionary. Similar to ToDictionary however this time the returned type is a MultiValueDictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelectorFunc">The key selector func.</param>
        /// <returns>MultiValueDictionary with the values of source stored under the keys retrieved by the keySelectorFunc which is applied to each
        /// value in source, or null if source is null</returns>
        public static MultiValueDictionary<TKey, TValue> ToMultiValueDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelectorFunc)
        {
            if (source == null)
            {
                return null;
            }

            var toReturn = new MultiValueDictionary<TKey, TValue>();
            foreach (TValue v in source)
            {
                toReturn.Add(keySelectorFunc(v), v);
            }
            return toReturn;
        }

        public static IEnumerable<TKey> KeysFromValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue val)
        {
            return dict.Keys.Where(k => dict[k].Equals(val));
        }
    }
}