using System;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public static class IPersistentTemplatedTypeInfoExtensions {
        public static void SetDefaultTemplate(this IPersistentTemplatedTypeInfo persistentMemberInfo, TemplateType templateType)
        {
            persistentMemberInfo.CodeTemplateInfo =
                (ICodeTemplateInfo)Activator.CreateInstance(TypesInfo.Instance.CodeTemplateInfoType, persistentMemberInfo.Session);

            ICodeTemplate defaultTemplate = CodeTemplateBuilder.CreateDefaultTemplate(templateType, persistentMemberInfo.Session,
                                                                                      TypesInfo.Instance.CodeTemplateType,
                                                                                      getProvider(persistentMemberInfo));
            persistentMemberInfo.CodeTemplateInfo.CodeTemplate = defaultTemplate;
        }
        static CodeDomProvider getProvider(IPersistentTemplatedTypeInfo persistentMemberInfo)
        {
            return persistentMemberInfo is IPersistentClassInfo
                       ? ((IPersistentClassInfo)persistentMemberInfo).PersistentAssemblyInfo.CodeDomProvider
                       : ((IPersistentMemberInfo)persistentMemberInfo).Owner.PersistentAssemblyInfo.CodeDomProvider;
        }

    }
}