using System.ComponentModel;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [Registrator(typeof(ITemplateInfo))]
    public class TemplateInfo : BaseObject,ITemplateInfo {
        string _name;
        string _templateCode;

        public TemplateInfo(Session session) : base(session) {
        }
        private PersistentTypeInfo _persistentTypeInfo;
        [Browsable(false)]
        [Association("PersistentTypeInfo-TemplateInfos")]
        public PersistentTypeInfo PersistentTypeInfo {
            get { return _persistentTypeInfo; }
            set { SetPropertyValue("PersistentTypeInfo", ref _persistentTypeInfo, value); }
        }
        [Association("TemplateInfo-CodeTemplateInfos")]
        public XPCollection<CodeTemplateInfo> CodeTemplateInfos {
            get { return GetCollection<CodeTemplateInfo>("CodeTemplateInfos"); }
        }
        #region ITemplateInfo Members
        [RuleRequiredField(null, DefaultContexts.Save)]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string TemplateCode {
            get { return _templateCode; }
            set { SetPropertyValue("TemplateCode", ref _templateCode, value); }
        }
        #endregion
    }

    
}