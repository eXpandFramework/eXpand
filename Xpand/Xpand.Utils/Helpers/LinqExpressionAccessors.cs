using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Xpand.Utils.Helpers {
    public static class LinqExpressionAccessors {
        public static Func<T, R> GenerateMemberGetter<T, R>(string member_name) {
            var param = Expression.Parameter(typeof(T), "this");
            var member = Expression.PropertyOrField(param, member_name);	// basically 'this.member_name'
            var lambda = Expression.Lambda<Func<T, R>>(member, param);
            return lambda.Compile();
        }

        public static Func<object, R> GenerateMemberGetter<R>(Type type, string member_name) {
            var param = Expression.Parameter(typeof(object), "this");
            var cast_param = Expression.Convert(param, type);						// '((type)this)'
            var member = Expression.PropertyOrField(cast_param, member_name);	// basically 'this.member_name'
            var lambda = Expression.Lambda<Func<object, R>>(member, param);
            return lambda.Compile();
        }

        /// <summary>Signature for a method which sets a specific member of a value type</summary>
        /// <typeparam name="T">Type of the value-type we're fondling</typeparam>
        /// <typeparam name="V">Type of the value we're setting</typeparam>
        /// <param name="this">The object instance to fondle</param>
        /// <param name="value">The new value to set the member to</param>
        public delegate void ValueTypeMemberSetterDelegate<T, in V>(ref T @this, V value);
        /// <summary>Signature for a method which sets a specific member of a reference type</summary>
        /// <typeparam name="T">Type of the reference-type we're fondling</typeparam>
        /// <typeparam name="V">Type of the value we're setting</typeparam>
        /// <param name="this">The object instance to fondle</param>
        /// <param name="value">The new value to set the member to</param>
        public delegate void ReferenceTypeMemberSetterDelegate<in T, in V>(T @this, V value);

        /// <summary>Generate a specific member setter for a specific value type</summary>
        /// <typeparam name="T">The type which contains the member</typeparam>
        /// <typeparam name="V">The member's actual type</typeparam>
        /// <param name="member_name">The member's name as defined in <typeparamref name="T"/></param>
        /// <returns>A compiled lambda which can access (set) the member</returns>
        public static ValueTypeMemberSetterDelegate<T, V> GenerateValueTypeMemberSetter<T, V>(string member_name)
            where T : struct {
            // Get a "ref type" of the value-type we're dealing with
            // Eg: Guid => "System.Guid&"
            var this_ref = typeof(T).MakeByRefType();

            var param_this = Expression.Parameter(this_ref, "this");
            var param_value = Expression.Parameter(typeof(V), "value");				// the member's new value
            var member = Expression.PropertyOrField(param_this, member_name);	// i.e., 'this.member_name'
            var assign = Expression.Assign(member, param_value);				// i.e., 'this.member_name = value'
            var lambda = Expression.Lambda<ValueTypeMemberSetterDelegate<T, V>>(assign, param_this, param_value);

            return lambda.Compile();
        }

        /// <summary>Generate a specific member setter for a specific reference type</summary>
        /// <typeparam name="T">The type which contains the member</typeparam>
        /// <typeparam name="V">The member's actual type</typeparam>
        /// <param name="member_name">The member's name as defined in <typeparamref name="T"/></param>
        /// <returns>A compiled lambda which can access (set) the member</returns>
        public static ReferenceTypeMemberSetterDelegate<T, V> GenerateReferenceTypeMemberSetter<T, V>(string member_name)
            where T : class {
            var param_this = Expression.Parameter(typeof(T), "this");
            var param_value = Expression.Parameter(typeof(V), "value");				// the member's new value
            var member = Expression.PropertyOrField(param_this, member_name);	// i.e., 'this.member_name'
            var assign = Expression.Assign(member, param_value);				// i.e., 'this.member_name = value'
            var lambda = Expression.Lambda<ReferenceTypeMemberSetterDelegate<T, V>>(assign, param_this, param_value);

            return lambda.Compile();
        }

        static readonly Dictionary<KeyValuePair<Type,string>,object> Delegates=new Dictionary<KeyValuePair<Type, string>, object>(); 
        public static ReferenceTypeMemberSetterDelegate<object, V> GenerateReferenceTypeMemberSetter<V>(this object obj, string member_name) {
            var type = obj.GetType();
            var keyValuePair = new KeyValuePair<Type, string>(type, member_name);
            if (!Delegates.ContainsKey(keyValuePair)) {
                var param_this = Expression.Parameter(typeof(object), "this");
                var param_value = Expression.Parameter(typeof(V), "value");				// the member's new value
                var cast_this = Expression.Convert(param_this, type);					// i.e., '((type)this)'
                var member = Expression.PropertyOrField(cast_this, member_name);	// i.e., 'this.member_name'
                var assign = Expression.Assign(member, param_value);				// i.e., 'this.member_name = value'
                var lambda = Expression.Lambda<ReferenceTypeMemberSetterDelegate<object, V>>(assign, param_this, param_value);
                Delegates.Add(keyValuePair, lambda.Compile());
            }
            return (ReferenceTypeMemberSetterDelegate<object, V>) Delegates[keyValuePair];
        }
    };

}