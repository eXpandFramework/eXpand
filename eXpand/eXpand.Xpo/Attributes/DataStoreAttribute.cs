using System;

namespace eXpand.Xpo.Attributes {
    [AttributeUsage(AttributeTargets.Assembly,AllowMultiple = true)]
    public class DataStoreAttribute:Attribute
    {
        readonly Type _nameSpaceType;
        readonly string _dataStoreNameSuffix;

        public DataStoreAttribute(Type nameSpaceType, string dataStoreNameSuffix) {
            _nameSpaceType = nameSpaceType;
            _dataStoreNameSuffix = dataStoreNameSuffix;
        }

        public string NameSpace {
            get { return _nameSpaceType.Namespace; }
        }

        public string DataStoreNameSuffix {
            get { return _dataStoreNameSuffix; }
        }
    }
}