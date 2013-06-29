using System;

namespace Xpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Interface,AllowMultiple = false,Inherited = false)]
    public class ModelEditorLogicRuleAttribute:Attribute {
        readonly Type _ruleType;

        public ModelEditorLogicRuleAttribute(Type ruleType) {
            _ruleType = ruleType;
        }

        public Type RuleType {
            get { return _ruleType; }
        }
    }
}
