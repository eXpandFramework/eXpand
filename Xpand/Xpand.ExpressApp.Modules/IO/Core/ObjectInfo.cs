using System;

namespace Xpand.ExpressApp.IO.Core {
    internal struct ObjectInfo {
        readonly Type _type;
        readonly object _key;

        public ObjectInfo(Type type,object key) {
            _type = type;
            _key = key;
        }

        public object Key {
            get { return _key; }
        }

        public Type Type {
            get { return _type; }
        }
    }
}