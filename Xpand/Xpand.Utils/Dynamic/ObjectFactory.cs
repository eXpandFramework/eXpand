using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Xpand.Utils.Dynamic {
    // This is optional class. Just helps in creating objects when
    // a cache is needed or not needed.
    public static class ObjectFactoryFactory<T, P1> where T : class {
        static readonly Hashtable cache = Hashtable.Synchronized(new Hashtable());

        public static Func<P1, T> CreateObject {
            get { return ObjectFactory<T, P1>.CreateObject; }
        }

        public static Func<P1, T> CreateObjectWithCache {
            get {
                return ObjectFactory<T, P1>.UseCache(cache);
            }
        }
    }

    // Main object creation factory class.
    public static class ObjectFactory<T, P1> where T : class {

        static Func<P1, T> _createObject;

        public static Func<P1, T> CreateObject {
            get {
                if (_createObject != null) return _createObject;
                _createObject = CreateDelegate();
                return _createObject;
            }
        }

        static Func<P1, T> CreateDelegate() {
            Type objType = typeof(T);
            var types = new[] { typeof(P1) };
            var dynMethod = new DynamicMethod("DM$OBJ_FACTORY_" +
                objType.Name, objType, types, objType);
            ILGenerator ilGen = dynMethod.GetILGenerator();
            // if need more than 1 arg add another Ldarg_x
            // you'll also need to add proper generics and 
            // CreateDelegate signatures
            ilGen.Emit(OpCodes.Ldarg_0);
            ConstructorInfo constructorInfo = objType.GetConstructor(types);
            Debug.Assert(constructorInfo != null, "constructorInfo != null");
            ilGen.Emit(OpCodes.Newobj, constructorInfo);
            ilGen.Emit(OpCodes.Ret);
            return (Func<P1, T>)dynMethod.CreateDelegate(typeof(Func<P1, T>));
        }

        public static Func<P1, T> UseCache(Hashtable cache) {
            Type t = typeof(T);
            var c = cache[t] as Func<P1, T>;
            if (c == null) {
                lock (cache.SyncRoot) {
                    c = cache[t] as Func<P1, T>;
                    if (c != null) {
                        return c;
                    }
                    c = CreateDelegate();
                    cache.Add(t, c);
                }

            }
            return c;
        }
    }
    public static class FastObjectFactory {
        private static readonly Hashtable CreatorCache = Hashtable.Synchronized(new Hashtable());
        private readonly static Type CoType = typeof(CreateObject);
        public delegate object CreateObject();

        /// <summary/>
        /// Create an object that will used as a 'factory' to the specified type T 
        /// <returns></returns>
        public static CreateObject CreateObjectFactory(Type t) {
            var c = CreatorCache[t] as CreateObject;
            if (c == null) {
                lock (CreatorCache.SyncRoot) {
                    c = CreatorCache[t] as CreateObject;
                    if (c != null) {
                        return c;
                    }
                    var dynMethod = new DynamicMethod("DM$OBJ_FACTORY_" + t.Name, typeof(object), null, t);
                    ILGenerator ilGen = dynMethod.GetILGenerator();

                    ilGen.Emit(OpCodes.Newobj, t.GetConstructor(Type.EmptyTypes));
                    ilGen.Emit(OpCodes.Ret);
                    c = (CreateObject)dynMethod.CreateDelegate(CoType);
                    CreatorCache.Add(t, c);
                }
            }
            return c;
        }

        public static CreateObject CreateObjectFactory<T>() where T : class {
            Type t = typeof(T);
            return CreateObjectFactory(t);
        }
    }
}
