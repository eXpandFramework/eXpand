using System;
using System.Reflection;

namespace Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public class AttributeInfoAttribute : Attribute {
        private readonly object[] _initializedArgumentValues;
        private readonly ConstructorInfo _constructor;

        public AttributeInfoAttribute() {
        }

        public AttributeInfoAttribute(ConstructorInfo constructorInfo, params object[] initializedArgumentValues) {
            _constructor = constructorInfo;
            _initializedArgumentValues = initializedArgumentValues;
        }


        public object[] InitializedArgumentValues {
            get { return _initializedArgumentValues; }
        }

        public ConstructorInfo Constructor {
            get {
                return _constructor;
            }

        }

        public object Instance { get; set; }
    }
}