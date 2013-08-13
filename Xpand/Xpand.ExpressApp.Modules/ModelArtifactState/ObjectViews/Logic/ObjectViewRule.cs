using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic {
    public class ObjectViewRule : LogicRule, IObjectViewRule {
        public ObjectViewRule(IObjectViewRule objectViewRule)
            : base(objectViewRule) {
            ObjectView = objectViewRule.ObjectView;
        }

        public IModelObjectView ObjectView { get; set; }
    }
}
