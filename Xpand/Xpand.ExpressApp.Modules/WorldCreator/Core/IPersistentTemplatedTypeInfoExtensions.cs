using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base.PersistentMetaData;
using Fasterflect;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public static class IPersistentTemplatedTypeInfoExtensions {
        public static void SetDefaultTemplate(this IPersistentTemplatedTypeInfo persistentMemberInfo, TemplateType templateType) {
            var objectType = WCTypesInfo.Instance.FindBussinessObjectType<ICodeTemplateInfo>();
            persistentMemberInfo.CodeTemplateInfo = (ICodeTemplateInfo)objectType.CreateInstance(new object[] { persistentMemberInfo.Session });

            var defaultTemplate = CodeTemplateBuilder.CreateDefaultTemplate(templateType, persistentMemberInfo.Session,
                                                                                      WCTypesInfo.Instance.FindBussinessObjectType<ICodeTemplate>(),
                                                                                      GetProvider(persistentMemberInfo));
            persistentMemberInfo.CodeTemplateInfo.CodeTemplate = defaultTemplate;
            persistentMemberInfo.CodeTemplateInfo.CloneProperties();
        }
        static CodeDomProvider GetProvider(IPersistentTemplatedTypeInfo persistentMemberInfo) {
            return persistentMemberInfo is IPersistentClassInfo
                       ? ((IPersistentClassInfo)persistentMemberInfo).PersistentAssemblyInfo.CodeDomProvider
                       : ((IPersistentMemberInfo)persistentMemberInfo).Owner.PersistentAssemblyInfo.CodeDomProvider;
        }

    }
}