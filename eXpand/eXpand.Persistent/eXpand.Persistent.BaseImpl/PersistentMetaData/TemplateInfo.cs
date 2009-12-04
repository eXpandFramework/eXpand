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
        [Association("TemplateInfo-PersistentTypeInfos")]
        public XPCollection<PersistentTypeInfo> TypeInfos
        {
            get
            {
                return GetCollection<PersistentTypeInfo>("TypeInfos");
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


        private string _usings;
        [Size(SizeAttribute.Unlimited)]
        public string Usings
        {
            get
            {
                return _usings;
            }
            set
            {
                SetPropertyValue("Usings", ref _usings, value);
            }
        }
        #endregion
    }
}