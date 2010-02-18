using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState
{
    public class ActionStateRuleAttribute : ArtifactRuleAttribute, IActionStateRule
    {
        private readonly string actionId;

        public ActionStateRuleAttribute(string id,string actionId, Nesting targetViewNesting, string normalCriteria,
                                        string emptyCriteria, ViewType viewType, string module, ActionState actionState, string viewId)
            : base(id, targetViewNesting, normalCriteria, emptyCriteria, viewType, module, viewId)
        {
            this.actionId = actionId;
            ActionState = actionState;
        }


        public string ActionId
        {
            get { return actionId; }
        }

        public ActionState ActionState { get; set; }

        string IActionStateRule.ActionId
        {
            get { return actionId; }
            set { throw new NotImplementedException(); }
        }
    }
}