using System;
using System.Collections.Generic;
using System.Linq;
using Xpand.Extensions.Mono.Cecil;

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
            var typeDefinition = AppDomain.CurrentDomain
                .GetAssemblies()
                .ToAssemblyDefinition()
                .SelectMany(definition => definition.MainModule.Types)
                .FirstOrDefault(definition => definition.FullName==nameSpaceType);
            if (typeDefinition!=null){
                var assembly = AppDomain.CurrentDomain.GetAssemblies().First(_ => _.FullName==typeDefinition.Module.Assembly.FullName);
                Dictionary[nameSpaceType] = assembly.GetType(typeDefinition.FullName);
                return typeDefinition.ToType();
            }
            return null;
        }

        public DataStoreAttribute(string connectionString, string nameSpaceType, bool isLegacy)
            : base(FindType(nameSpaceType), null, isLegacy) {
            ConnectionString = connectionString;
        }
    }
}