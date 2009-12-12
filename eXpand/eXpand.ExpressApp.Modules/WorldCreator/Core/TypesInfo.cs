using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Persistent.Base;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public class TypesInfo  {
        internal TypesInfo() {
            
        }
        private static IValueManager<TypesInfo> instanceManager;

        public static TypesInfo Instance {
            get {
                if (instanceManager == null) {
                    instanceManager = ValueManager.CreateValueManager<TypesInfo>();
                }
                return instanceManager.Value ?? (instanceManager.Value = new TypesInfo());
            }
        }

        public void AddTypes(IEnumerable<Type> types)
        {
            CodeTemplateInfoType = GetInfoType(types, typeof(ICodeTemplateInfo));
            TemplateInfoType = GetInfoType(types, typeof(ITemplateInfo));
            CodeTemplateType = GetInfoType(types, typeof(ICodeTemplate));
            PersistentAssemblyInfoType = GetInfoType(types, typeof(IPersistentAssemblyInfo));
            PersistentCoreTypeInfoType = GetInfoType(types, typeof(IPersistentCoreTypeMemberInfo));
            PersistentTypesInfoType = GetInfoType(types, typeof(IPersistentClassInfo));
            PersistentReferenceInfoType = GetInfoType(types, typeof(IPersistentReferenceMemberInfo));
            ExtendedCollectionMemberInfoType = GetInfoType(types, typeof(IExtendedCollectionMemberInfo));
            ExtendedReferenceMemberInfoType = GetInfoType(types, typeof(IExtendedReferenceMemberInfo));
            ExtendedCoreMemberInfoType = GetInfoType(types, typeof(IExtendedCoreTypeMemberInfo));
            IntefaceInfoType = GetInfoType(types, typeof(IInterfaceInfo));
        }
        private Type GetInfoType(IEnumerable<Type> types, Type type1) {
            var infoType = types.Where(type => type1.IsAssignableFrom(type)).GroupBy(type => type).Select(grouping => grouping.Key).FirstOrDefault();
            if (infoType== null)
                throw new NoNullAllowedException("No Class that implemenets " + type1.AssemblyQualifiedName + " found at AdditionalBusinessClasses list");
            return infoType;
        }

        public Type PersistentTypesInfoType { get; private set; }


        public Type PersistentAssemblyInfoType { get; private set; }
        public Type CodeTemplateInfoType { get; private set; }
        public Type PersistentCoreTypeInfoType { get; private set; }
        public Type PersistentReferenceInfoType { get; private set; }
        public Type ExtendedReferenceMemberInfoType { get; private set; }
        public Type ExtendedCollectionMemberInfoType { get; private set; }    
        public Type ExtendedCoreMemberInfoType { get; private set; }
        public Type IntefaceInfoType { get; private set; }
        public Type CodeTemplateType { get; private set; }
        public Type TemplateInfoType { get; private set; }
    }
}