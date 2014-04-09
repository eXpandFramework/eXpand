using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.Validation.RuleType;

namespace FeatureCenter.Module.Validation {

    [RuleCombinationOfPropertiesIsUnique("aaa", DefaultContexts.Save, "WarningProperty,Information")]
    public class RuleTypeCustomer : CustomerBase {
        public RuleTypeCustomer(Session session)
            : base(session) {
        }
        private string _warningProperty;
        [RuleRequiredField("Required_WarningProperty_for_Warning_Customer", DefaultContexts.Save)]
        public string WarningProperty {
            get {
                return _warningProperty;
            }
            set {
                SetPropertyValue("WarningProperty", ref _warningProperty, value);
            }
        }
        private string _information;
        [RuleRequiredField("Required_InformationProperty_for_Warning_Customer", DefaultContexts.Save)]
        public string Information {
            get {
                return _information;
            }
            set {
                SetPropertyValue("Information", ref _information, value);
            }
        }
        private string _infoOnControlValueChanged;

        [RuleValueComparison("RuleStringComparison_InfoOnControlValueChanged_for_Warning_Customer",
            RuleTypeController.ObjectSpaceObjectChanged, ValueComparisonType.NotEquals, "value")]
        [ImmediatePostData]
        public string InfoOnControlValueChanged {
            get { return _infoOnControlValueChanged; }
            set { SetPropertyValue("InfoOnControlValueChanged", ref _infoOnControlValueChanged, value); }
        }

    }


}
