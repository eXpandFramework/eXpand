using System;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class TypeInfoExtensions {
        public static void Init(this IPersistentTypeInfo persistentTypeInfo, Type codeTemplateType) {
            if (persistentTypeInfo as IPersistentMemberInfo!= null)
                ((IPersistentMemberInfo) persistentTypeInfo).Init(codeTemplateType);
            else
                ((IPersistentClassInfo) persistentTypeInfo).Init(codeTemplateType);
        }

        public static void Init(this IPersistentMemberInfo persistentMemberInfo, Type codeTemplateType) {
            CodeTemplateBuilder.CreateDefaultTemplate(
                persistentMemberInfo is IPersistentCollectionMemberInfo
                    ? TemplateType.ReadOnlyMember
                    : TemplateType.ReadWriteMember, persistentMemberInfo, codeTemplateType);
        }

        public static void Init(this IPersistentClassInfo persistentClassInfo, Type codeTemplateType) {
            CodeTemplateBuilder.CreateDefaultTemplate(TemplateType.Class, persistentClassInfo, codeTemplateType);
        }
    }
}