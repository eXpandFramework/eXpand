using System;
using System.ComponentModel;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [InterfaceRegistrator(typeof(IPersistentRuleRequiredFieldAttribute))]
    [DefaultProperty("DefaultProperty")]
    [System.ComponentModel.DisplayName("Rule Required")]
    [CreateableItem(typeof(IPersistentMemberInfo))]
    [CreateableItem(typeof(IExtendedMemberInfo))]
    public class PersistentRuleRequiredFieldAttribute : PersistentAttributeInfo, IPersistentRuleRequiredFieldAttribute {
        string _context;
        string _iD;


        public PersistentRuleRequiredFieldAttribute(Session session)
            : base(session) {
        }

        public string ID {
            get { return _iD; }
            set { SetPropertyValue("ID", ref _iD, value); }
        }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string DefaultProperty {
            get { return string.Format("{0}: {1}", typeof(RuleRequiredFieldAttribute).Name, ID); }
        }
        [RuleRequiredField]
        public string Context {
            get { return _context; }
            set { SetPropertyValue("Context", ref _context, value); }
        }
        private string _targetCriteria;

        [Size(SizeAttribute.Unlimited)]
        [AttributeInfo]
        public string TargetCriteria {
            get {
                return _targetCriteria;
            }
            set {
                SetPropertyValue("TargetCriteria", ref _targetCriteria, value);
            }
        }
        public override AttributeInfoAttribute Create() {
            return
                new AttributeInfoAttribute(
                    typeof(RuleRequiredFieldAttribute).GetConstructor(new[] { typeof(string), typeof(string) }), ID,
                    Context) { Instance = this };
        }
    }
}