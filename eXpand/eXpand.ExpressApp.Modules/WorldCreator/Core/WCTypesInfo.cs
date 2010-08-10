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
        }

        public static WCTypesInfo Instance
        {
            get { return _instance; }
        }
        public Type FindBussinessObjectType<T>() {
            return _dictionary[typeof(T)];
        }

        public void Register(IEnumerable<Type> types) {
            IEnumerable<Type> persistentTypes = GetPersistentTypes(types);
            _dictionary = new Dictionary<Type,Type>();
            foreach (var persistentType in persistentTypes) {
                var interfaceType = persistentType.GetCustomAttributes(typeof(RegistratorAttribute),false).OfType<RegistratorAttribute>().Single().InterfaceType;
                _dictionary.Add(interfaceType,persistentType);
            }
        }

        IEnumerable<Type> GetPersistentTypes(IEnumerable<Type> types) {
            return types.Where(type => type.GetCustomAttributes(typeof(RegistratorAttribute),false).OfType<RegistratorAttribute>().Count()==1);
        }
    }
}