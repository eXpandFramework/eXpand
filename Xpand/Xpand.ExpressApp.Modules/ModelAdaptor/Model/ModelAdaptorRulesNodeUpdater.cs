using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.ModelAdapter.Logic;

namespace Xpand.ExpressApp.ModelAdaptor.Model {
    public class ModelAdaptorRulesNodeUpdater :LogicRulesNodeUpdater<IModelAdaptorRule, IModelModelAdaptorRule> {
        protected override void SetAttribute(IModelModelAdaptorRule rule, IModelAdaptorRule attribute) {
            rule.Attribute = attribute;
        }

    }
}