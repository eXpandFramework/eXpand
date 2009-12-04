using System;
using System.IO;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.WorldCreator {
    public static class TypeInfoExtensions {
        public static void Init(this IPersistentTypeInfo persistentTypeInfo, Type codeTemplateType) {
            if (persistentTypeInfo as IPersistentMemberInfo!= null)
                ((IPersistentMemberInfo) persistentTypeInfo).Init(codeTemplateType);
            else
                ((IPersistentClassInfo) persistentTypeInfo).Init(codeTemplateType);
        }

        public static void Init(this IPersistentMemberInfo persistentMemberInfo, Type codeTemplateType) {
            createDefaultTemplate(TemplateType.Member, persistentMemberInfo, codeTemplateType);
        }

        public static void Init(this IPersistentClassInfo persistentClassInfo, Type codeTemplateType) {
            createDefaultTemplate(TemplateType.Class, persistentClassInfo, codeTemplateType);
        }

        static void createDefaultTemplate(TemplateType templateType, IPersistentTypeInfo persistentClassInfo, Type codeTemplateType) {
            const ICodeTemplate template = null;
            var defaultTemplate = findDefaultTemplate(template, templateType, persistentClassInfo, codeTemplateType);
            if (defaultTemplate== null) {
                defaultTemplate = (ICodeTemplate)Activator.CreateInstance(codeTemplateType, persistentClassInfo.Session);
                defaultTemplate.IsDefault = true;
                defaultTemplate.TemplateType=templateType;
                defaultTemplate.TemplateCode = GetFromResource("eXpand.ExpressApp.WorldCreator.Resources.Default" +templateType+ "Templates.xml");
                defaultTemplate.References = GetFromResource("eXpand.ExpressApp.WorldCreator.Resources.Default" + templateType + "References.xml");
            }
            persistentClassInfo.CodeTemplate=defaultTemplate;
        }


        static string GetFromResource(string name) {
            var manifestResourceStream = typeof (TypeInfoExtensions).Assembly.GetManifestResourceStream(
                name);
            if (manifestResourceStream != null)
                return new StreamReader(manifestResourceStream).ReadToEnd();
            return null;
        }

        static ICodeTemplate findDefaultTemplate(ICodeTemplate template, TemplateType templateType, IPersistentTypeInfo persistentClassInfo, Type codeTemplateType) {
            var binaryOperator = new BinaryOperator(template.GetPropertyName(x => x.TemplateType),templateType);
            var @operator = new BinaryOperator(template.GetPropertyName(x => x.IsDefault),true);
            return persistentClassInfo.Session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction,
                                                       codeTemplateType, new GroupOperator(binaryOperator, @operator))
                as ICodeTemplate;
        }
    }

}