using System.ComponentModel;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [InterfaceRegistrator(typeof(IPersistentRuleRequiredFieldAttribute))]
    [DefaultProperty("DefaultProperty")]
    public class PersistentRuleRequiredFieldAttribute : PersistentAttributeInfo, IPersistentRuleRequiredFieldAttribute {
        string _context;
        string _iD;


        public PersistentRuleRequiredFieldAttribute(Session session) : base(session) {
        }

        public string ID {
            get { return _iD; }
            set { SetPropertyValue("ID", ref _iD, value); }
        }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string DefaultProperty {
            get { return string.Format("{0}: {1}", typeof (RuleRequiredFieldAttribute).Name, ID); }
        }

        public string Context {
            get { return _context; }
            set { SetPropertyValue("Context", ref _context, value); }
        }

        public override AttributeInfo Create() {
            return
                new AttributeInfo(
                    typeof (RuleRequiredFieldAttribute).GetConstructor(new[] {typeof (string), typeof (string)}), ID,
                    Context);
        }
    }
}