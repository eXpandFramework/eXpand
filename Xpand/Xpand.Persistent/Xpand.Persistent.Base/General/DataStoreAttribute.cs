using System;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DataStoreAttribute : Xpand.Xpo.DB.DataStoreAttribute {
        public DataStoreAttribute(Type nameSpaceType, string dataStoreName)
            : base(nameSpaceType, dataStoreName) {
        }

        public DataStoreAttribute(Type nameSpaceType, string dataStoreName, bool isLegacy)
            : base(nameSpaceType, dataStoreName, isLegacy) {
        }

        public DataStoreAttribute(string connectionString, string nameSpaceType)
            : base(ReflectionHelper.FindType(nameSpaceType), null) {
            _connectionString = connectionString;
        }
        public DataStoreAttribute(string connectionString, string nameSpaceType, bool isLegacy)
            : base(ReflectionHelper.FindType(nameSpaceType), null, isLegacy) {
            _connectionString = connectionString;
        }
    }
}