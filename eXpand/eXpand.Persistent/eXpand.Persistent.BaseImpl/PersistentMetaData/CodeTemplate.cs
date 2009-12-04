using System.IO;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    public class CodeTemplate:TemplateInfo, ICodeTemplate {
        public CodeTemplate(Session session) : base(session) {
        }

        private TemplateType _templateType;
        public void SetDefaults()
        {
            TemplateCode = GetFromResource(@"eXpand.Persistent.BaseImpl.PersistentMetaData.Resources.Default" + TemplateType + @"Templates.xml");
            Usings = GetFromResource(@"eXpand.Persistent.BaseImpl.PersistentMetaData.Resources.Default" + TemplateType + @"Usings.xml");
            Name = "Default";
        }
        
        string GetFromResource(string name)
        {
            var manifestResourceStream = GetType().Assembly.GetManifestResourceStream(
                name);
            if (manifestResourceStream != null)
            {
                using (var streamReader = new StreamReader(manifestResourceStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
            return null;
        }
        
        [RuleValueComparison(null,DefaultContexts.Save, ValueComparisonType.GreaterThan, 0)]
        public TemplateType TemplateType
        {
            get
            {
                return _templateType;
            }
            set
            {
                SetPropertyValue("TemplateType", ref _templateType, value);
            }
        }

        private bool _isDefault;

        public bool IsDefault
        {
            get
            {
                return _isDefault;
            }
            set
            {
                SetPropertyValue("IsDefault", ref _isDefault, value);
            }
        }
    }
}