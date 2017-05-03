using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.MasterDetail;
using Xpand.Utils.Linq;

namespace Xpand.ExpressApp.MasterDetail.Logic {
    public class MasterDetailRuleController : ViewController {
        List<IMasterDetailRule> _masterDetailRules ;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            _masterDetailRules = new List<IMasterDetailRule>();
            Frame.Disposing+=FrameOnDisposing;
            Frame.GetController<LogicRuleViewController>(controller => {
                controller.LogicRuleExecutor.LogicRuleExecute += LogicRuleViewControllerOnLogicRuleExecute;
                var masterDetailViewControllerBase = Frame.Controllers.Values.OfType<IMasterDetailViewController>().SingleOrDefault();
                if (masterDetailViewControllerBase != null)
                    masterDetailViewControllerBase.RequestRules = frame1 => {
                        var masterDetailRules = frame1.GetController<MasterDetailRuleController>()._masterDetailRules.DistinctBy(rule => rule.Id);
                        return masterDetailRules.Select(rule => new MasterDetailRuleInfo(rule.ChildListView, rule.CollectionMember, rule.TypeInfo, null)).ToList();
                    };
            });
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs){
            Frame.GetController<LogicRuleViewController>(controller => controller.LogicRuleExecutor.LogicRuleExecute -=
                LogicRuleViewControllerOnLogicRuleExecute);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _masterDetailRules?.Clear();
        }

        void LogicRuleViewControllerOnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs logicRuleExecuteEventArgs) {
            var info = logicRuleExecuteEventArgs.LogicRuleInfo;
            var masterDetailRule = info.Rule as IMasterDetailRule;
            if (masterDetailRule!=null) {
                if (info.Active) {
                    if (!(_masterDetailRules.Contains(masterDetailRule))) {
                        _masterDetailRules.Add(masterDetailRule);
                    }
                }
                else {
                    _masterDetailRules.Remove(masterDetailRule);
                }
            }
        }

        public List<IMasterDetailRule> MasterDetailRules => _masterDetailRules;
    }

}