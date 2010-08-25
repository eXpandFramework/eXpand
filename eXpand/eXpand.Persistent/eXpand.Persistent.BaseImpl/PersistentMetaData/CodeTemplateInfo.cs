using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [InterfaceRegistrator(typeof(ICodeTemplateInfo))]
    public class CodeTemplateInfo : BaseObject, ICodeTemplateInfo {
        CodeTemplate _codeTemplate;

        TemplateInfo _templateInfo;

        public CodeTemplateInfo(Session session) : base(session) {
        }
        [NonPersistent]
        [VisibleInListView(false)]
        public CodeTemplate CodeTemplate {
            get { return _codeTemplate; }
            set { SetPropertyValue("CodeTemplate", ref _codeTemplate, value); }
        }

        [VisibleInListView(false)]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [Aggregated]
        [Association("TemplateInfo-CodeTemplateInfos")]
        public TemplateInfo TemplateInfo {
            get { return _templateInfo; }
            set { SetPropertyValue("TemplateInfo", ref _templateInfo, value); }
        }


        #region ICodeTemplateInfo Members

        ITemplateInfo ICodeTemplateInfo.TemplateInfo {
            get { return TemplateInfo; }
            set { TemplateInfo = value as TemplateInfo; }
        }

        ICodeTemplate ICodeTemplateInfo.CodeTemplate {
            get { return CodeTemplate; }
            set { CodeTemplate = value as CodeTemplate; }
        }
        #endregion
        [Association("CodeTemplateInfo-PersistentTemplatedTypeInfos")]
        public XPCollection<PersistentTemplatedTypeInfo> PersistentTemplatedTypeInfos
        {
            get
            {
                return GetCollection<PersistentTemplatedTypeInfo>("PersistentTemplatedTypeInfos");
            }
        }
    }
}