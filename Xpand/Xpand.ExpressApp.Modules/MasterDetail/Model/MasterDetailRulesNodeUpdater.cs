using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.MasterDetail.Logic;

namespace Xpand.ExpressApp.MasterDetail.Model {
    public class MasterDetailRulesNodeUpdater :LogicRulesNodeUpdater<IMasterDetailRule, IModelMasterDetailRule> {
        protected override void SetAttribute(IModelMasterDetailRule rule,IMasterDetailRule attribute) {
            rule.Attribute = attribute;
        }
    }
}