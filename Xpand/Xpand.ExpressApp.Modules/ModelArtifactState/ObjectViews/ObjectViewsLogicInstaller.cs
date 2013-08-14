using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic;
using Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews {
    public class ObjectViewsLogicInstaller : LogicInstaller<IObjectViewRule, IModelObjectViewRule> {
        public ObjectViewsLogicInstaller(XpandModuleBase xpandModuleBase)
            : base(xpandModuleBase) {

        }

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext>{
                    ExecutionContext.CustomizeShowViewParameters , ExecutionContext.CurrentObjectChanged ,
                    ExecutionContext.CustomProcessSelectedItem
                };
            }
        }

        public override LogicRulesNodeUpdater<IObjectViewRule, IModelObjectViewRule> LogicRulesNodeUpdater {
            get { return new ObjectViewRulesNodeUpdater(); }
        }

        public override IModelLogic GetModelLogic(IModelApplication modelApplication) {
            return ((IModelApplicationConditionalObjectView) modelApplication).ConditionalObjectView;
        }

    }
}