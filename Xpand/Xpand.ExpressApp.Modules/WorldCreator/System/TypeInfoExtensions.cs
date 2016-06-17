using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.System {
    public static class TypeInfoExtensions {

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