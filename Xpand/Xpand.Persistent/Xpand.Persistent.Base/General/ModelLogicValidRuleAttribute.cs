using System;
using DevExpress.ExpressApp.Utils;
using Xpand.Persistent.Base.Logic;

namespace Xpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Interface,AllowMultiple = false,Inherited = false)]
    public sealed class ModelLogicValidRuleAttribute:Attribute {
        readonly Type _ruleType;

        public ModelLogicValidRuleAttribute(Type ruleType) {
            Guard.TypeArgumentIs(typeof(ILogicRule), ruleType, "ruleType");
            _ruleType = ruleType;
        }

        public Type RuleType {
            get { return _ruleType; }
        }
    }
}
