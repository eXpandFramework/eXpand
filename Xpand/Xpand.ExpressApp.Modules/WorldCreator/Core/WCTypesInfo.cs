using System;
using System.Collections.Generic;
using System.Linq;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public class WCTypesInfo {
        static readonly WCTypesInfo _instance;
        Dictionary<Type, Type> _dictionary;

        static WCTypesInfo() {
            _instance = new WCTypesInfo();
            Instance._dictionary = new Dictionary<Type, Type>();
        }

        public static WCTypesInfo Instance {
            get { return _instance; }
        }
        public Type FindBussinessObjectType(Type type) {
            Type findBussinessObjectType;
            try {
                findBussinessObjectType = _dictionary[type];
            } catch (KeyNotFoundException) {
                throw new KeyNotFoundException("Register " + type + " at your AdditionalBusinessClasses");
            }
            return findBussinessObjectType;
        }

        public Type FindBussinessObjectType<T>() {
            return FindBussinessObjectType(typeof(T));
        }

        public void Reset() {
            _dictionary.Clear();
        }
        public void Register(IEnumerable<Type> types) {
            IEnumerable<Type> persistentTypes = GetPersistentTypes(types);
            foreach (var persistentType in persistentTypes) {
                var interfaceType = persistentType.GetCustomAttributes(typeof(InterfaceRegistratorAttribute), false).OfType<InterfaceRegistratorAttribute>().Single().InterfaceType;
                if (!_dictionary.ContainsKey(interfaceType))
                    _dictionary.Add(interfaceType, persistentType);
            }
        }

        IEnumerable<Type> GetPersistentTypes(IEnumerable<Type> types) {
            return types.Where(type => type.GetCustomAttributes(typeof(InterfaceRegistratorAttribute), false).OfType<InterfaceRegistratorAttribute>().Count() == 1);
        }

        public void Register(Type type) {
            Register(new List<Type> { type });
        }
    }
}