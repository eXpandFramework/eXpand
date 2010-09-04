using System.Reflection;

namespace Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos {
    public class AttributeInfo {
        private readonly object[] _initializedArgumentValues;
        private readonly ConstructorInfo _constructor;

        public AttributeInfo(ConstructorInfo constructorInfo, params object[] initializedArgumentValues)
        {
            _constructor=constructorInfo;
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
    }
}