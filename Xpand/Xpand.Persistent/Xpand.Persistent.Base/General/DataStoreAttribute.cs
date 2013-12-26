using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DataStoreAttribute : Xpand.Xpo.DB.DataStoreAttribute {
        static readonly Dictionary<string,Type> _dictionary=new Dictionary<string, Type>(); 
        public DataStoreAttribute(Type nameSpaceType, string dataStoreName)
            : base(nameSpaceType, dataStoreName) {
        }

        public DataStoreAttribute(Type nameSpaceType, string dataStoreName, bool isLegacy)
            : base(nameSpaceType, dataStoreName, isLegacy) {
        }

        public DataStoreAttribute(string connectionString, string nameSpaceType)
            : base(FindType(nameSpaceType), null) {
            _connectionString = connectionString;
        }

        private static Type FindType(string nameSpaceType){
            if (_dictionary.ContainsKey(nameSpaceType))
                return _dictionary[nameSpaceType];
            var type = ReflectionHelper.FindType(nameSpaceType);
            if (type != null)
                _dictionary[nameSpaceType] = type;
            return type;
        }

        public DataStoreAttribute(string connectionString, string nameSpaceType, bool isLegacy)
            : base(ReflectionHelper.FindType(nameSpaceType), null, isLegacy) {
            _connectionString = connectionString;
        }
    }
}