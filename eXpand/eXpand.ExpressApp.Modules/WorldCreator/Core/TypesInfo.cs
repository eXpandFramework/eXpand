using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Persistent.Base;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public interface ITypesInfo {
        Type PersistentTypesInfoType { get; }
        Type PersistentAssemblyInfoType { get; }
        Type CodeTemplateInfoType { get; }
        Type PersistentCoreTypeInfoType { get; }
        Type PersistentReferenceInfoType { get; }
        Type ExtendedReferenceMemberInfoType { get; }
        Type ExtendedCollectionMemberInfoType { get; }
        Type ExtendedCoreMemberInfoType { get; }
        Type IntefaceInfoType { get; }
        Type CodeTemplateType { get; }
        Type TemplateInfoType { get; }
    }

    public class TypesInfo : ITypesInfo {
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
            PersistentAssociationAttributeType = GetInfoType(types, typeof(IPersistentAssociationAttribute));
            PersistentDefaultClassOptionsAttributeType = GetInfoType(types, typeof(IPersistentDefaulClassOptionsAttribute));
            CodeTemplateType = GetInfoType(types, typeof(ICodeTemplate));
            PersistentAssemblyInfoType = GetInfoType(types, typeof(IPersistentAssemblyInfo));
            PersistentCoreTypeInfoType = GetInfoType(types, typeof(IPersistentCoreTypeMemberInfo));
            PersistentTypesInfoType = GetInfoType(types, typeof(IPersistentClassInfo));
            PersistentReferenceInfoType = GetInfoType(types, typeof(IPersistentReferenceMemberInfo));
            PersistentCollectionInfoType = GetInfoType(types, typeof(IPersistentCollectionMemberInfo));
            ExtendedCollectionMemberInfoType = GetInfoType(types, typeof(IExtendedCollectionMemberInfo));
            ExtendedReferenceMemberInfoType = GetInfoType(types, typeof(IExtendedReferenceMemberInfo));
            ExtendedCoreMemberInfoType = GetInfoType(types, typeof(IExtendedCoreTypeMemberInfo));
            IntefaceInfoType = GetInfoType(types, typeof(IInterfaceInfo));
        }
        private Type GetInfoType(IEnumerable<Type> types, Type type1) {
            var infoType = types.Where(type1.IsAssignableFrom).GroupBy(type => type).Select(grouping => grouping.Key).FirstOrDefault();
            if (infoType== null)
                throw new NoNullAllowedException("No Class that implemenets " + type1.AssemblyQualifiedName +
                                                 " found at AdditionalBusinessClasses list. " + typeof(WorldCreatorModule).Name +
                                                 " should be the last module added to Application.Modules. Please check InitializeComponent method of your XafApplication descenant");
            return infoType;
        }

        public Type PersistentTypesInfoType { get; private set; }


        public Type PersistentAssemblyInfoType { get; private set; }
        public Type CodeTemplateInfoType { get; private set; }
        public Type PersistentCoreTypeInfoType { get; private set; }
        public Type PersistentCollectionInfoType { get; private set; }
        public Type PersistentReferenceInfoType { get; private set; }
        public Type ExtendedReferenceMemberInfoType { get; private set; }
        public Type ExtendedCollectionMemberInfoType { get; private set; }    
        public Type ExtendedCoreMemberInfoType { get; private set; }
        public Type IntefaceInfoType { get; private set; }
        public Type CodeTemplateType { get; private set; }
        public Type TemplateInfoType { get; private set; }
        public Type PersistentAssociationAttributeType { get; private set; }
        public Type PersistentDefaultClassOptionsAttributeType { get; private set; }

        
    }
}