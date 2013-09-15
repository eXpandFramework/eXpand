using System.Linq;
using Xpand.Persistent.Base.PersistentMetaData;
using Fasterflect;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public static class ICodeTemplateInfoExtensions {
        public static void CloneProperties(this ICodeTemplateInfo codeTemplateInfo) {
            var templateInfo = codeTemplateInfo.TemplateInfo ?? CreateWcObject(codeTemplateInfo);
            codeTemplateInfo.TemplateInfo = templateInfo;
            var infos = typeof(ITemplateInfo).GetProperties().Select(propertyInfo =>
                                                                     new {
                                                                         Value = propertyInfo.GetValue(codeTemplateInfo.CodeTemplate, null),
                                                                         PropertyInfo = propertyInfo
                                                                     });
            foreach (var info in infos) {
                templateInfo.SetPropertyValue(info.PropertyInfo.Name, info.Value);
            }
        }

        static ITemplateInfo CreateWcObject(ICodeTemplateInfo codeTemplateInfo) {
            var type = WCTypesInfo.Instance.FindBussinessObjectType(typeof(ITemplateInfo));
            return (ITemplateInfo)type.CreateInstance(new object[] { codeTemplateInfo.Session });
        }
    }
}
