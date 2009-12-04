using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    public class CodeTemplate:BaseObject, ICodeTemplate {
        public CodeTemplate(Session session) : base(session) {
        }

        private TemplateType _templateType;

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

        private string _templateCode;

        [Size(SizeAttribute.Unlimited)]
        [RuleRequiredField(null,DefaultContexts.Save)]
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

        private string _references;

        [Size(SizeAttribute.Unlimited)]
        public string References
        {
            get
            {
                return _references;
            }
            set
            {
                SetPropertyValue("References", ref _references, value);
            }
        }

        [Association("CodeTemplate-PersistentTypeInfos")]
        public XPCollection<PersistentTypeInfo> TypeInfos
        {
            get
            {
                return GetCollection<PersistentTypeInfo>("TypeInfos");
            }
        }

        IList<IPersistentTypeInfo> ICodeTemplate.TypeInfos {
            get {
                return new ListConverter<IPersistentTypeInfo,PersistentTypeInfo>(TypeInfos);
            }
        }
    }
}