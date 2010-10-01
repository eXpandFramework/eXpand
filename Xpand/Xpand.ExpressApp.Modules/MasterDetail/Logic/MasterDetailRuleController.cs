using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.MasterDetail.Model;

namespace Xpand.ExpressApp.MasterDetail.Logic {
    public class MasterDetailRuleController : ConditionalLogicRuleViewController<IMasterDetailRule> {
        readonly List<IMasterDetailRule> _masterDetailRules = new List<IMasterDetailRule>();

        protected override void OnDeactivating() {
            base.OnDeactivating();
            _masterDetailRules.Clear();
        }

        protected override IModelLogic GetModelLogic() {
            return ((IModelApplicationMasterDetail)Application.Model).MasterDetail;
        }

        protected override IEnumerable<LogicRuleInfo<IMasterDetailRule>> GetContextValidLogicRuleInfos(View view, IEnumerable<IMasterDetailRule> modelLogicRules, object currentObject, ExecutionContext executionContext, bool invertCustomization) {
            var contextValidLogicRuleInfos = base.GetContextValidLogicRuleInfos(view, modelLogicRules, currentObject, executionContext, invertCustomization);
            if (contextValidLogicRuleInfos.Count() > 0)
                _masterDetailRules.Clear();
            return contextValidLogicRuleInfos;
        }

        public override void ExecuteRule(LogicRuleInfo<IMasterDetailRule> info, ExecutionContext executionContext) {

            if (info.Active) {
                if (!(_masterDetailRules.Contains(info.Rule)))
                    _masterDetailRules.Add(info.Rule);
            } else {
                _masterDetailRules.Remove(info.Rule);
            }

        }
        public List<IMasterDetailRule> MasterDetailRules {
            get { return _masterDetailRules; }
        }

    }
}