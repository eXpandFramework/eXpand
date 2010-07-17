using System.Collections.Generic;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Conditional.Logic;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.MasterDetail.Model;

namespace eXpand.ExpressApp.MasterDetail.Logic {
    public class MasterDetailRuleController : ConditionalLogicRuleViewController<IMasterDetailRule>
    {
        readonly List<IMasterDetailRule> _masterDetailRules=new List<IMasterDetailRule>() ;
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            _masterDetailRules.Clear();
        }

        public override void ExecuteRule(LogicRuleInfo<IMasterDetailRule> info, ExecutionContext executionContext) {
            if (info.Active) {
                if (!(_masterDetailRules.Contains(info.Rule)))
                    _masterDetailRules.Add(info.Rule);
            }
            else {
                _masterDetailRules.Remove(info.Rule);
            }
        }
        public List<IMasterDetailRule> MasterDetailRules
        {
            get { return _masterDetailRules; }
        }

        protected override IModelGroupContexts GetModelGroupContexts(string executionContextGroup) {
            return ((IModelApplicationMasterDetail)Application.Model).MasterDetail.GroupContexts;
        }
    }
}