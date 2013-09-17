using System;

namespace Xpand.ExpressApp.Validation.RuleType {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class RuleTypeAttribute : Attribute {
        readonly string _id;
        readonly RuleType _ruleType;

        public RuleTypeAttribute(string id, RuleType ruleType) {
            _id = id;
            _ruleType = ruleType;
        }

        public RuleType RuleType {
            get { return _ruleType; }
        }

        public string Id {
            get { return _id; }
        }
    }
}
