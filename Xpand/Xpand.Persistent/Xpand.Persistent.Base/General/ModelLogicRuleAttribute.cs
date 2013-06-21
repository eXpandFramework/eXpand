using System;

namespace Xpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Interface,AllowMultiple = false,Inherited = false)]
    public class ModelLogicRuleAttribute:Attribute {
        readonly Type _ruleType;

        public ModelLogicRuleAttribute(Type ruleType) {
            _ruleType = ruleType;
        }

        public Type RuleType {
            get { return _ruleType; }
        }
    }
}
