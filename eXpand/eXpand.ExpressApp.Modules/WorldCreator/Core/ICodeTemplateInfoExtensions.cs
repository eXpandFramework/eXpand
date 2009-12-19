using System.Linq;
using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core
{
    public static class ICodeTemplateInfoExtensions
    {
        public static void CloneProperties(this ICodeTemplateInfo codeTemplateInfo)
        {
            var templateInfo = (ITemplateInfo)(codeTemplateInfo.TemplateInfo ?? ObjectSpace.FindObjectSpace(codeTemplateInfo).CreateObject(TypesInfo.Instance.TemplateInfoType));
            codeTemplateInfo.TemplateInfo = templateInfo;
            var type = templateInfo.GetType();
            var infos = typeof(ITemplateInfo).GetProperties().Select(propertyInfo =>
                                                                     new
                                                                     {
                                                                         Value = propertyInfo.GetValue(codeTemplateInfo.CodeTemplate, null),
                                                                         TypeInfoPropertyInfo = type.GetProperty(propertyInfo.Name)
                                                                     });
            foreach (var info in infos)
            {
                info.TypeInfoPropertyInfo.SetValue(templateInfo, info.Value, null);
            }
        }

    }
}
