using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {

    
    public class CodeTemplateInfo:BaseObject, ICodeTemplateInfo {
        public CodeTemplateInfo(Session session) : base(session) {
        }

        private CodeTemplate _codeTemplate;
        
        private TemplateInfo _templateInfo;
        [RuleRequiredField(null, DefaultContexts.Save)]
        public CodeTemplate CodeTemplate
        {
            get
            {
                return _codeTemplate;
            }
            set
            {
                SetPropertyValue("CodeTemplate", ref _codeTemplate, value);
            }
        }
        [VisibleInListView(false)]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [Aggregated]
        [Association("TemplateInfo-CodeTemplateInfos")]
        public TemplateInfo TemplateInfo
        {
            get
            {
                return _templateInfo;
            }
            set
            {
                SetPropertyValue("TemplateInfo", ref _templateInfo, value);
            }
        }

        private string _generatedCode;
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute,"false")]
        [Size(SizeAttribute.Unlimited)]
        public string GeneratedCode
        {
            get
            {
                return _generatedCode;
            }
            set
            {
                SetPropertyValue("GeneratedCode", ref _generatedCode, value);
            }
        }
        private PersistentAssemblyInfo _persistentAssemblyInfo;
        [Association("PersistentAssemblyInfo-CodeTemplateInfos")]
        public PersistentAssemblyInfo PersistentAssemblyInfo
        {
            get
            {
                return _persistentAssemblyInfo;
            }
            set
            {
                SetPropertyValue("PersistentAssemblyInfo", ref _persistentAssemblyInfo, value);
            }
        }
        IPersistentAssemblyInfo ICodeTemplateInfo.PersistentAssemblyInfo {
            get { return PersistentAssemblyInfo; }
            set { PersistentAssemblyInfo=value as PersistentAssemblyInfo; }
        }

        ITemplateInfo ICodeTemplateInfo.TemplateInfo {
            get { return TemplateInfo; }
            set { TemplateInfo=value as TemplateInfo; }
        }

        ICodeTemplate ICodeTemplateInfo.CodeTemplate {
            get { return CodeTemplate; }
            set { CodeTemplate=value as CodeTemplate; }
        }

        
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            TemplateInfo = new TemplateInfo(Session);
        }

    }
}