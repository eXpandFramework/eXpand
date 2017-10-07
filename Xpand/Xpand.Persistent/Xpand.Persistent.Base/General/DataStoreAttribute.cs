using System;
using System.Collections.Generic;
using System.Linq;

namespace Xpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class DataStoreAttribute : Xpand.Xpo.DB.DataStoreAttribute {
        static readonly Dictionary<string,Type> Dictionary=new Dictionary<string, Type>(); 
        public DataStoreAttribute(Type nameSpaceType, string dataStoreName)
            : base(nameSpaceType, dataStoreName) {
        }

        public DataStoreAttribute(Type nameSpaceType, string dataStoreName, bool isLegacy)
            : base(nameSpaceType, dataStoreName, isLegacy) {
        }

        public DataStoreAttribute(string connectionString, string nameSpaceType)
            : base(FindType(nameSpaceType), null,true) {
            ConnectionString = connectionString;
        }

        private static Type FindType(string nameSpaceType){
            if (Dictionary.ContainsKey(nameSpaceType))
                return Dictionary[nameSpaceType];
            var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type1 => type1.FullName==nameSpaceType);
            if (type != null)
                Dictionary[nameSpaceType] = type;
            return type;
        }

        public DataStoreAttribute(string connectionString, string nameSpaceType, bool isLegacy)
            : base(FindType(nameSpaceType), null, isLegacy) {
            ConnectionString = connectionString;
        }
    }
}