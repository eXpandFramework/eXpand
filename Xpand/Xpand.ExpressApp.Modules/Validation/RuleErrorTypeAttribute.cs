using System;

namespace Xpand.ExpressApp.Validation {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class RuleErrorTypeAttribute : Attribute {
        readonly string _id;
        readonly RuleType _ruleType;

        public RuleErrorTypeAttribute(string id, RuleType ruleType) {
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
