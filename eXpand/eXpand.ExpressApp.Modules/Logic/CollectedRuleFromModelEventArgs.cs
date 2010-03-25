using System;
using System.Collections.Generic;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Logic {
    public class CollectedRuleFromModelEventArgs<TLogicRule> : EventArgs where TLogicRule:ILogicRule{
        readonly List<TLogicRule> _logicRules;

        public CollectedRuleFromModelEventArgs(List<TLogicRule> logicRules) {
            _logicRules = logicRules;
        }

        public List<TLogicRule> LogicRules {
            get { return _logicRules; }
        }
    }
}