using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    public class TemplateInfo :BaseObject, ITemplateInfo {
        public TemplateInfo(Session session) : base(session) {
        }
        [Association("TemplateInfo-CodeTemplateInfos")]
        public XPCollection<CodeTemplateInfo> CodeTemplateInfos
        {
            get
            {
                return GetCollection<CodeTemplateInfo>("CodeTemplateInfos");
            }
        }
        private string _name;
        
        [RuleRequiredField(null, DefaultContexts.Save)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetPropertyValue("Name", ref _name, value);
            }
        }
        #region ITemplateInfo Members
        
        private string _templateCode;
        [Size(SizeAttribute.Unlimited)]
        public string TemplateCode
        {
            get
            {
                return _templateCode;
            }
            set
            {
                SetPropertyValue("TemplateCode", ref _templateCode, value);
            }
        }



        #endregion
    }
}