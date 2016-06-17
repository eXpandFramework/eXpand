using DevExpress.ExpressApp;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.BusinessObjects {
    public static class PersistentTemplatedTypeInfoExtensions {
        public static void SetDefaultTemplate(this IPersistentTemplatedTypeInfo persistentMemberInfo, TemplateType templateType) {
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<ICodeTemplateInfo>();
            persistentMemberInfo.CodeTemplateInfo = (ICodeTemplateInfo)objectType.CreateInstance(persistentMemberInfo.Session);
            persistentMemberInfo.CodeTemplateInfo.CreateDefaultTemplate(templateType, GetProvider(persistentMemberInfo));
            persistentMemberInfo.CodeTemplateInfo.CloneProperties();
        }
        static CodeDomProvider GetProvider(IPersistentTemplatedTypeInfo persistentMemberInfo) {
            return (persistentMemberInfo as IPersistentClassInfo)?.PersistentAssemblyInfo.CodeDomProvider ?? ((IPersistentMemberInfo)persistentMemberInfo).Owner.PersistentAssemblyInfo.CodeDomProvider;
        }

    }
}