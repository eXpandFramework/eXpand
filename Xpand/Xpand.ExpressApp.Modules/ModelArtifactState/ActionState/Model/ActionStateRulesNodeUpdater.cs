using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Model {
    public class ActionStateRulesNodeUpdater :
        LogicRulesNodeUpdater<IActionStateRule, IModelActionStateRule> {
        protected override void SetAttribute(IModelActionStateRule rule,IActionStateRule attribute) {
            rule.Attribute = attribute;
        }

    }
}