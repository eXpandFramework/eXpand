using System;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public class TypeInfo {
        private readonly Type _type;
        private readonly IPersistentClassInfo _persistentClassInfo;

        public TypeInfo(Type type, IPersistentClassInfo persistentClassInfo) {
            _type = type;
            _persistentClassInfo = persistentClassInfo;
        }

        public IPersistentClassInfo PersistentClassInfo {
            get { return _persistentClassInfo; }
        }

        public Type Type {
            get { return _type; }
        }
    }
}