using System;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers {

    public class CodeTemplateBuilder {
        public static ICodeTemplate CreateDefaultTemplate(TemplateType templateType, Session session, Type codeTemplateType, CodeDomProvider codeDomProvider) {
            
            var defaultTemplate = CodeTemplateQuery.FindDefaultTemplate(templateType, session, codeTemplateType,codeDomProvider);
            if (defaultTemplate== null) {
                defaultTemplate = (ICodeTemplate)ReflectionHelper.CreateObject(codeTemplateType, session);
                defaultTemplate.IsDefault = true;
                defaultTemplate.TemplateType=templateType;
                defaultTemplate.CodeDomProvider=codeDomProvider;
                defaultTemplate.SetDefaults();
            }
            return defaultTemplate;
            
        }
    }
}