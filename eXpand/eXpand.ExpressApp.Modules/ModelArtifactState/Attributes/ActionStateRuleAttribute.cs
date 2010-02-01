using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes
{
    public enum ActionState {
        Default=0,
        Disabled=1,
        Hidden=2,
        Executed=3,
        ExecutedAndDisable=4
    }
    public class ActionStateRuleAttribute:ArtifactStateRuleAttribute,IActionStateRule
    {
        private readonly string actionId;

        public ActionStateRuleAttribute(string id,string actionId, Nesting targetViewNesting, string normalCriteria,
                                        string emptyCriteria, ViewType viewType, string module, ActionState state, string viewId)
            : base(id, targetViewNesting, normalCriteria, emptyCriteria, viewType, module, (State) state,viewId)
        {
            this.actionId = actionId;
        }

        public new ActionState State
        { get; set; }

        public string ActionId
        {
            get { return actionId; }
        }
        string IActionStateRule.ActionId
        {
            get { return actionId; }
            set { throw new NotImplementedException(); }
        }
    }
}