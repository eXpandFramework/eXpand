using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Validation;

namespace FeatureCenter.Module.Validation {

    [RuleCombinationOfPropertiesIsUnique("aaa", DefaultContexts.Save, "WarningProperty,Information")]
    [RuleErrorType("aaa", RuleType.Warning)]
    public class RuleTypeCustomer : Customer {
        public RuleTypeCustomer(Session session)
            : base(session) {
        }
        private string _warningProperty;
        [RuleRequiredField("Required_WarningProperty_for_Warning_Customer", DefaultContexts.Save)]
        [RuleErrorType("Required_WarningProperty_for_Warning_Customer", RuleType.Warning)]
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
        [RuleErrorType("Required_InformationProperty_for_Warning_Customer", RuleType.Information)]
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
            WarningController.ObjectSpaceObjectChanged, ValueComparisonType.NotEquals, "value")]
        [RuleErrorType("RuleStringComparison_InfoOnControlValueChanged_for_Warning_Customer", RuleType.Information)]
        [ImmediatePostData]
        public string InfoOnControlValueChanged {
            get { return _infoOnControlValueChanged; }
            set { SetPropertyValue("InfoOnControlValueChanged", ref _infoOnControlValueChanged, value); }
        }

    }


}
