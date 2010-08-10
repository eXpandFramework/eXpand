using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class TypeInfoExtensions {
        public static void SetTypeValue(this IPersistentTypeInfo persistentTypeInfo, ref IPersistentClassInfo persistentClassInfo, ref Type type, string typeFullName) {
            var session = persistentTypeInfo.Session;
            if (!string.IsNullOrEmpty(typeFullName)){
                IPersistentClassInfo classInfo = PersistentClassInfoQuery.Find(session, typeFullName);
                if (classInfo != null)
                    persistentClassInfo = classInfo;
                else
                    type = ReflectionHelper.GetType(typeFullName.Substring(typeFullName.LastIndexOf(".") + 1));
            }
        }

        public static void Init(this IPersistentTemplatedTypeInfo persistentTemplatedTypeInfo, Type codeTemplateType) {
            persistentTemplatedTypeInfo.CodeTemplateInfo = ObjectSpaceExtensions.CreateWCObject<ICodeTemplateInfo>(ObjectSpace.FindObjectSpace(persistentTemplatedTypeInfo));
            if (persistentTemplatedTypeInfo is IPersistentMemberInfo)
            {
                var persistentMemberInfo = ((IPersistentMemberInfo)persistentTemplatedTypeInfo);
                persistentMemberInfo.Init(codeTemplateType,persistentMemberInfo.Owner.PersistentAssemblyInfo.CodeDomProvider);
            }
            else if (persistentTemplatedTypeInfo is IPersistentClassInfo) {
                var persistentClassInfo = ((IPersistentClassInfo) persistentTemplatedTypeInfo);
                persistentClassInfo.Init(codeTemplateType,persistentClassInfo.PersistentAssemblyInfo.CodeDomProvider);
            }
        }

        public static void Init(this IPersistentMemberInfo persistentMemberInfo, Type codeTemplateType, CodeDomProvider codeDomProvider)
        {
            persistentMemberInfo.CodeTemplateInfo.CodeTemplate =CodeTemplateBuilder.CreateDefaultTemplate(
                persistentMemberInfo is IPersistentCollectionMemberInfo
                    ? TemplateType.XPCollectionMember
                    : TemplateType.XPReadWritePropertyMember, persistentMemberInfo.Session, codeTemplateType,codeDomProvider);
        }

        public static void Init(this IPersistentClassInfo persistentClassInfo, Type codeTemplateType, CodeDomProvider codeDomProvider) {
            
            persistentClassInfo.CodeTemplateInfo.CodeTemplate = CodeTemplateBuilder.CreateDefaultTemplate(TemplateType.Class, persistentClassInfo.Session, codeTemplateType, codeDomProvider);
        }
    }
}