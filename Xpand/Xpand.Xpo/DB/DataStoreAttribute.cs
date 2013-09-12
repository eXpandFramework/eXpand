using System;

namespace Xpand.Xpo.DB {
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DataStoreAttribute : Attribute {
        protected string _connectionString;
        readonly Type _nameSpaceType;
        readonly string _dataStoreName;
        readonly bool _isLegacy;

        public DataStoreAttribute(Type nameSpaceType, string dataStoreName) {
            _nameSpaceType = nameSpaceType;
            _dataStoreName = dataStoreName;
        }

        public DataStoreAttribute(Type nameSpaceType, string dataStoreName, bool isLegacy) {
            _nameSpaceType = nameSpaceType;
            _dataStoreName = dataStoreName;
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

        public string DataStoreName {
            get { return _dataStoreName; }
        }
    }
}