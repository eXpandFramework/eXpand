using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.WorldCreator.BusinessObjects {
    public static class CodeTemplateInfoExtensions {
        public static void CloneProperties(this ICodeTemplateInfo codeTemplateInfo) {
            var templateInfo = codeTemplateInfo.TemplateInfo ?? Create(codeTemplateInfo);
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

        static ITemplateInfo Create(ICodeTemplateInfo codeTemplateInfo) {
            var type = XafTypesInfo.Instance.FindBussinessObjectType<ITemplateInfo>();
            return (ITemplateInfo)type.CreateInstance(codeTemplateInfo.Session);
        }

        private static ICodeTemplate FindDefaultTemplate(TemplateType templateType, Session session, Type codeTemplateType, CodeDomProvider codeDomProvider) {
            const ICodeTemplate template = null;
            var binaryOperator = new BinaryOperator(template.GetPropertyName(x => x.TemplateType), templateType);
            var isDefault = new BinaryOperator(template.GetPropertyName(x => x.IsDefault), true);
            var provider = new BinaryOperator(template.GetPropertyName(x => x.CodeDomProvider), codeDomProvider);
            return session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction,
                                      codeTemplateType, new GroupOperator(binaryOperator, isDefault, provider))
                   as ICodeTemplate;
        }

        public static void CreateDefaultTemplate(this ICodeTemplateInfo codeTemplateInfo,TemplateType templateType,  CodeDomProvider codeDomProvider) {
            var codeTemplateType = XafTypesInfo.Instance.FindBussinessObjectType<ICodeTemplate>();
            var session = codeTemplateInfo.Session;
            var defaultTemplate = FindDefaultTemplate(templateType, session, codeTemplateType, codeDomProvider);
            if (defaultTemplate == null) {
                defaultTemplate = (ICodeTemplate)codeTemplateType.CreateInstance(session);
                defaultTemplate.IsDefault = true;
                defaultTemplate.TemplateType = templateType;
                defaultTemplate.CodeDomProvider = codeDomProvider;
                defaultTemplate.SetDefaults();
            }
            codeTemplateInfo.CodeTemplate=defaultTemplate;

        }

    }
}
