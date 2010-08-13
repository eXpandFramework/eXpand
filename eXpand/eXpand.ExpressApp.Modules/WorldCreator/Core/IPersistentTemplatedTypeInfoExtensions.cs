using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class IPersistentTemplatedTypeInfoExtensions {
        public static void SetDefaultTemplate(this IPersistentTemplatedTypeInfo persistentMemberInfo, TemplateType templateType)
        {
            var objectSpace = ObjectSpace.FindObjectSpace(persistentMemberInfo);
            persistentMemberInfo.CodeTemplateInfo =objectSpace.CreateWCObject<ICodeTemplateInfo>();

            ICodeTemplate defaultTemplate = CodeTemplateBuilder.CreateDefaultTemplate(templateType, persistentMemberInfo.Session,
                                                                                      WCTypesInfo.Instance.FindBussinessObjectType<ICodeTemplate>(),
                                                                                      getProvider(persistentMemberInfo));
            persistentMemberInfo.CodeTemplateInfo.CodeTemplate = defaultTemplate;
            persistentMemberInfo.CodeTemplateInfo.CloneProperties();
        }
        static CodeDomProvider getProvider(IPersistentTemplatedTypeInfo persistentMemberInfo)
        {
            return persistentMemberInfo is IPersistentClassInfo
                       ? ((IPersistentClassInfo)persistentMemberInfo).PersistentAssemblyInfo.CodeDomProvider
                       : ((IPersistentMemberInfo)persistentMemberInfo).Owner.PersistentAssemblyInfo.CodeDomProvider;
        }

    }
}