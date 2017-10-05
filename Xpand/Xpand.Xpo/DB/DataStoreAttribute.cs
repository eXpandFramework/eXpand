using System;

namespace Xpand.Xpo.DB {
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DataStoreAttribute : Attribute {
        
        readonly Type _nameSpaceType;

        public DataStoreAttribute(Type nameSpaceType, string dataStoreName) {
            _nameSpaceType = nameSpaceType;
            DataStoreName = dataStoreName;
        }

        public DataStoreAttribute(Type nameSpaceType, string dataStoreName, bool isLegacy) {
            _nameSpaceType = nameSpaceType;
            DataStoreName = dataStoreName;
            IsLegacy = isLegacy;
        }

        public bool IsLegacy{ get; }

        public string ConnectionString{ get; protected set; }

        public string NameSpace => _nameSpaceType.Namespace;

        public string DataStoreName{ get; }
    }
}