using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Persistent.Base;
using eXpand.Persistent.Base.ImportExport;

namespace eXpand.ExpressApp.ImportExport.Core {
    public class TypesInfo {
        static IValueManager<TypesInfo> instanceManager;

        internal TypesInfo() {
        }

        public static TypesInfo Instance {
            get {
                if (instanceManager == null) {
                    instanceManager = ValueManager.CreateValueManager<TypesInfo>();
                }
                return instanceManager.Value ?? (instanceManager.Value = new TypesInfo());
            }
        }

        public Type ClassInfoGraphNodeType { get; private set; }
        public Type SerializationConfigurationType { get; private set; }

        public void AddTypes(IEnumerable<Type> types) {
            SerializationConfigurationType = GetInfoType(types, typeof (ISerializationConfiguration));
            ClassInfoGraphNodeType = GetInfoType(types, typeof(IClassInfoGraphNode));
        }

        Type GetInfoType(IEnumerable<Type> types, Type type1) {
            Type infoType =
                types.Where(type => type1.IsAssignableFrom(type)).GroupBy(type => type).Select(grouping => grouping.Key)
                    .FirstOrDefault();
            if (infoType == null)
                throw new NoNullAllowedException("No Class that implemenets " + type1.AssemblyQualifiedName +
                                                 " found at AdditionalBusinessClasses list");
            return infoType;
        }
    }
}