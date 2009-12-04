using System;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers {
    public class CodeTemplateBuilder {
        public static void CreateDefaultTemplate(TemplateType templateType, IPersistentTypeInfo persistentClassInfo, Type codeTemplateType) {
            
            var defaultTemplate = CodeTemplateQuery.FindDefaultTemplate(templateType, persistentClassInfo.Session, codeTemplateType);
            if (defaultTemplate== null) {
                defaultTemplate = (ICodeTemplate)Activator.CreateInstance(codeTemplateType, persistentClassInfo.Session);
                defaultTemplate.IsDefault = true;
                defaultTemplate.TemplateType=templateType;
            }
            persistentClassInfo.CodeTemplate=defaultTemplate;
        }
    }
}