using System;
using System.Linq;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public static class ICodeTemplateInfoExtensions {
        public static void CloneProperties(this ICodeTemplateInfo codeTemplateInfo) {
            var templateInfo = codeTemplateInfo.TemplateInfo ?? CreateWcObject(codeTemplateInfo);
            codeTemplateInfo.TemplateInfo = templateInfo;
            var type = templateInfo.GetType();
            var infos = typeof(ITemplateInfo).GetProperties().Select(propertyInfo =>
                                                                     new {
                                                                         Value = propertyInfo.GetValue(codeTemplateInfo.CodeTemplate, null),
                                                                         TypeInfoPropertyInfo = type.GetProperty(propertyInfo.Name)
                                                                     });
            foreach (var info in infos) {
                info.TypeInfoPropertyInfo.SetValue(templateInfo, info.Value, null);
            }
        }

        static ITemplateInfo CreateWcObject(ICodeTemplateInfo codeTemplateInfo) {
            var type = WCTypesInfo.Instance.FindBussinessObjectType(typeof(ITemplateInfo));
            return (ITemplateInfo) Activator.CreateInstance(type,new object[]{codeTemplateInfo.Session});
        }
    }
}
