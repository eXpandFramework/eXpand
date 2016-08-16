using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.ModelArtifact;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic {
    public class ObjectViewRule : LogicRule, IObjectViewRule {
        public ObjectViewRule(IContextObjectViewRule objectViewRule)
            : base(objectViewRule) {
            ObjectView = objectViewRule.ObjectView;
        }

        public IModelObjectView ObjectView { get; set; }
    }
}
