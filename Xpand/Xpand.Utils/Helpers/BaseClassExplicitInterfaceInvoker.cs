using System;
using System.Collections.Generic;
using System.Reflection;

namespace Xpand.Utils.Helpers{
    public class BaseClassExplicitInterfaceInvoker<T> {
        private readonly Dictionary<string, MethodInfo> _cache = new Dictionary<string, MethodInfo>();
        private readonly Type _baseType = typeof(T).BaseType;

        private MethodInfo FindMethod(string methodName) {
            MethodInfo method;
            if (!_cache.TryGetValue(methodName, out method)) {
                var methods = _baseType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var methodInfo in methods) {
                    if (methodInfo.IsFinal && methodInfo.IsPrivate) {
                        if (methodInfo.Name == methodName || methodInfo.Name.EndsWith("." + methodName)) {
                            method = methodInfo;
                            break;
                        }
                    }
                }

                _cache.Add(methodName, method);
            }

            return method;
        }

        public TRt Invoke<TRt>(T obj, string methodName, object[] paramaters=null) {
            MethodInfo method = FindMethod(methodName);
            return (TRt)method.Invoke(obj, paramaters);
        }

    }
}