using System.Collections.Generic;
using System.Linq;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Linq;

namespace Xpand.ExpressApp.MasterDetail.Logic {
    public class MasterDetailRuleController : ConditionalLogicRuleViewController<IMasterDetailRule,MasterDetailModule> {
        readonly List<IMasterDetailRule> _masterDetailRules = new List<IMasterDetailRule>();
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            var masterDetailViewControllerBase = Frame.Controllers.Values.OfType<IMasterDetailViewController>().Single();
            masterDetailViewControllerBase.RequestRules = frame1 => {
                IEnumerable<IMasterDetailRule> masterDetailRules = frame1.GetController<MasterDetailRuleController>()._masterDetailRules.DistinctBy(rule => rule.Id);
                return masterDetailRules.Select(rule => new MasterDetailRuleInfo(rule.ChildListView, rule.CollectionMember, rule.TypeInfo, null, rule.SynchronizeActions)).ToList();
            };
        }

        public override void ExecuteRule(LogicRuleInfo<IMasterDetailRule> info, ExecutionContext executionContext) {
            if (info.Active) {
                if (!(_masterDetailRules.Contains(info.Rule))) {
                    _masterDetailRules.Add(info.Rule);
                }
            } else {
                _masterDetailRules.Remove(info.Rule);
            }
        }

        public List<IMasterDetailRule> MasterDetailRules {
            get { return _masterDetailRules; }
        }

    }

}