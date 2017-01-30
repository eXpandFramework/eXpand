using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.System {
    public static class WCExtensions {

        public static List<IPersistentAssemblyInfo> GetModifiedPersistentAssemblies(this IObjectSpace objectSpace){
            var persistentAssemblyInfos = new List<IPersistentAssemblyInfo>();
            foreach (var modifiedObject in objectSpace.ModifiedObjects) {
                var persistentAssemblyInfo = modifiedObject as IPersistentAssemblyInfo;
                if (persistentAssemblyInfo != null) persistentAssemblyInfos.Add(persistentAssemblyInfo);
                var persistentClassInfo = modifiedObject as IPersistentClassInfo;
                if (persistentClassInfo != null)
                    persistentAssemblyInfos.Add(persistentClassInfo.PersistentAssemblyInfo);
                var persistentMemberInfo = modifiedObject as IPersistentMemberInfo;
                if (persistentMemberInfo != null)
                    persistentAssemblyInfos.Add(persistentMemberInfo.Owner.PersistentAssemblyInfo);
                var persistentAttributeInfo = modifiedObject as IPersistentAttributeInfo;
                if (persistentAttributeInfo != null) {
                    persistentClassInfo = persistentAttributeInfo.Owner as IPersistentClassInfo;
                    if (persistentClassInfo != null)
                        persistentAssemblyInfos.Add(persistentClassInfo.PersistentAssemblyInfo);
                    persistentMemberInfo = persistentAttributeInfo.Owner as IPersistentMemberInfo;
                    if (persistentMemberInfo != null)
                        persistentAssemblyInfos.Add(persistentMemberInfo.Owner.PersistentAssemblyInfo);
                }
                var templateInfo = modifiedObject as ITemplateInfo;
                if (templateInfo != null){
                    var persistentTemplatedTypeInfos =((IEnumerable<ICodeTemplateInfo>)objectSpace.TypesInfo.FindTypeInfo(templateInfo.GetType()).FindMember("CodeTemplateInfos").GetValue(templateInfo)).SelectMany(info 
                            => (IEnumerable<IPersistentTemplatedTypeInfo>)objectSpace.TypesInfo.FindTypeInfo(info.GetType()).FindMember("PersistentTemplatedTypeInfos").GetValue(info));
                    foreach (var persistentTemplatedTypeInfo in persistentTemplatedTypeInfos){
                        persistentClassInfo = persistentTemplatedTypeInfo as IPersistentClassInfo;
                        if (persistentClassInfo != null)
                            persistentAssemblyInfos.Add(persistentClassInfo.PersistentAssemblyInfo);
                        var memberInfo = persistentTemplatedTypeInfo as IPersistentMemberInfo;
                        if (memberInfo != null)
                            persistentAssemblyInfos.Add(memberInfo.Owner.PersistentAssemblyInfo);
                    }   
                }
            }
            return persistentAssemblyInfos;
        }

        public static void Init(this IPersistentTemplatedTypeInfo persistentTemplatedTypeInfo, Type codeTemplateType) {
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<ICodeTemplateInfo>();
            persistentTemplatedTypeInfo.CodeTemplateInfo = (ICodeTemplateInfo) Activator.CreateInstance(objectType,persistentTemplatedTypeInfo.Session);
            var templatedTypeInfo = persistentTemplatedTypeInfo as IPersistentMemberInfo;
            if (templatedTypeInfo != null) {
                var persistentMemberInfo = templatedTypeInfo;
                persistentMemberInfo.Init(codeTemplateType, persistentMemberInfo.Owner.PersistentAssemblyInfo.CodeDomProvider);
            } else if (persistentTemplatedTypeInfo is IPersistentClassInfo) {
                var persistentClassInfo = ((IPersistentClassInfo)persistentTemplatedTypeInfo);
                persistentClassInfo.Init(codeTemplateType, persistentClassInfo.PersistentAssemblyInfo.CodeDomProvider);
            }
        }

        public static void Init(this IPersistentMemberInfo persistentMemberInfo, Type codeTemplateType, CodeDomProvider codeDomProvider) {
            persistentMemberInfo.CodeTemplateInfo.CreateDefaultTemplate(persistentMemberInfo is IPersistentCollectionMemberInfo
                    ? TemplateType.XPCollectionMember
                    : TemplateType.XPReadWritePropertyMember, codeDomProvider);
        }

        public static void Init(this IPersistentClassInfo persistentClassInfo, Type codeTemplateType, CodeDomProvider codeDomProvider) {
            persistentClassInfo.CodeTemplateInfo.CreateDefaultTemplate(TemplateType.Class,  codeDomProvider);
        }
    }
}