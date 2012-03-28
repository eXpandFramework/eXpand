using System;

namespace Xpand.Xpo.DB {
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DataStoreAttribute : Attribute {
        protected string _connectionString;
        readonly Type _nameSpaceType;
        readonly string _dataStoreNameSuffix;
        readonly bool _isLegacy;

        public DataStoreAttribute(Type nameSpaceType, string dataStoreNameSuffix) {
            _nameSpaceType = nameSpaceType;
            _dataStoreNameSuffix = dataStoreNameSuffix;
        }

        public DataStoreAttribute(Type nameSpaceType, string dataStoreNameSuffix, bool isLegacy) {
            _nameSpaceType = nameSpaceType;
            _dataStoreNameSuffix = dataStoreNameSuffix;
            _isLegacy = isLegacy;
        }

        public bool IsLegacy {
            get { return _isLegacy; }
        }

        public string ConnectionString {
            get { return _connectionString; }
        }
        public string NameSpace {
            get { return _nameSpaceType.Namespace; }
        }

        public string DataStoreNameSuffix {
            get { return _dataStoreNameSuffix; }
        }
    }
}