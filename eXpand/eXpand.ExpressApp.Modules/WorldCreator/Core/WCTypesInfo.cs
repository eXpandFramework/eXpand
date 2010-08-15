using System;
using System.Collections.Generic;
using System.Linq;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public class WCTypesInfo
    {
        static readonly WCTypesInfo _instance;
        Dictionary<Type, Type> _dictionary;

        static WCTypesInfo() {
            _instance=new WCTypesInfo();
            Instance._dictionary = new Dictionary<Type, Type>();
        }

        public static WCTypesInfo Instance
        {
            get { return _instance; }
        }
        public Type FindBussinessObjectType<T>() {
            Type findBussinessObjectType;
            try {
                findBussinessObjectType = _dictionary[typeof(T)];
            }
            catch (KeyNotFoundException) {
                throw new KeyNotFoundException("Register " + typeof(T) + " at your AdditionalBusinessClasses");
            }
            return findBussinessObjectType;
        }

        public void Reset()
        {
            _dictionary.Clear();    
        }
        public void Register(IEnumerable<Type> types) {
            IEnumerable<Type> persistentTypes = GetPersistentTypes(types);
            foreach (var persistentType in persistentTypes) {
                var interfaceType = persistentType.GetCustomAttributes(typeof(WCRegistratorAttribute),false).OfType<WCRegistratorAttribute>().Single().InterfaceType;
                if (!_dictionary.ContainsKey(interfaceType))
                    _dictionary.Add(interfaceType,persistentType);
            }
        }

        IEnumerable<Type> GetPersistentTypes(IEnumerable<Type> types) {
            return types.Where(type => type.GetCustomAttributes(typeof(WCRegistratorAttribute),false).OfType<WCRegistratorAttribute>().Count()==1);
        }

        public void Register(Type type) {
            Register(new List<Type>{type});
        }
    }
}