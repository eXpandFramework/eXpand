using System.IO;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    [InterfaceRegistrator(typeof(ICodeTemplate))]
    public class CodeTemplate : TemplateInfo, ICodeTemplate {
        CodeDomProvider _codeDomProvider;
        bool _isDefault;
        TemplateType _templateType;

        public CodeTemplate(Session session) : base(session) {
        }
        #region ICodeTemplate Members
        public void SetDefaults() {
            TemplateCode =
                GetFromResource(@"Xpand.Persistent.BaseImpl.PersistentMetaData.Resources.Default" +CodeDomProvider+ TemplateType +
                                @"Templates.xml");
            Name = "Default";
        }
        [VisibleInLookupListView(true)]
        public CodeDomProvider CodeDomProvider {
            get { return _codeDomProvider; }
            set { SetPropertyValue("CodeDomProvider", ref _codeDomProvider, value); }
        }

        [VisibleInLookupListView(true)]
        [RuleValueComparison(null, DefaultContexts.Save, ValueComparisonType.NotEquals, TemplateType.None)]
        public TemplateType TemplateType {
            get { return _templateType; }
            set { SetPropertyValue("TemplateType", ref _templateType, value); }
        }

        public bool IsDefault {
            get { return _isDefault; }
            set { SetPropertyValue("IsDefault", ref _isDefault, value); }
        }
        #endregion
        string GetFromResource(string name) {
            Stream manifestResourceStream = GetType().Assembly.GetManifestResourceStream(
                name);
            if (manifestResourceStream != null) {
                using (var streamReader = new StreamReader(manifestResourceStream)) {
                    return streamReader.ReadToEnd();
                }
            }
            return null;
        }
    }
}