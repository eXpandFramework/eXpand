using DevExpress.ExpressApp;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public static class IPersistentTemplatedTypeInfoExtensions {
        public static void SetDefaultTemplate(this IPersistentTemplatedTypeInfo persistentMemberInfo, TemplateType templateType) {
            var objectSpace = ObjectSpace.FindObjectSpaceByObject(persistentMemberInfo);
            persistentMemberInfo.CodeTemplateInfo = objectSpace.CreateWCObject<ICodeTemplateInfo>();

            ICodeTemplate defaultTemplate = CodeTemplateBuilder.CreateDefaultTemplate(templateType, persistentMemberInfo.Session,
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