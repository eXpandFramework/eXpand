using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Validation;

namespace FeatureCenter.Module.Validation {

    [RuleCombinationOfPropertiesIsUnique("aaa", DefaultContexts.Save, "WarningProperty1,WarningProperty2")]
    [RuleWarning("aaa")]
    public class WarningCustomer : Customer {
        public WarningCustomer(Session session)
            : base(session) {
        }
        private string _warningProperty;
        [RuleRequiredField("Required_WarningProperty_for_Warning_Customer", DefaultContexts.Save)]
        [RuleWarning("Required_WarningProperty_for_Warning_Customer")]
        public string WarningProperty {
            get {
                return _warningProperty;
            }
            set {
                SetPropertyValue("WarningProperty", ref _warningProperty, value);
            }
        }
        private string _warningProperty1;
        [RuleRequiredField("Required_WarningProperty_for_Warning_Customer1", DefaultContexts.Save)]
        [RuleWarning("Required_WarningProperty_for_Warning_Customer1")]
        public string WarningProperty1 {
            get {
                return _warningProperty1;
            }
            set {
                SetPropertyValue("WarningProperty1", ref _warningProperty1, value);
            }
        }
        private string _warningProperty2;

        [RuleRequiredField("Required_WarningProperty_for_Warning_Customer2", DefaultContexts.Save)]
        public string WarningProperty2 {
            get {
                return _warningProperty2;
            }
            set {
                SetPropertyValue("WarningProperty2", ref _warningProperty2, value);
            }
        }

    }


}
