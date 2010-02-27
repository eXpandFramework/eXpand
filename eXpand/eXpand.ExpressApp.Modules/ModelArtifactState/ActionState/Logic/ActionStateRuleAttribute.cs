using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.ArtifactState.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public class ActionStateRuleAttribute : ArtifactRuleAttribute, IActionStateRule {
        readonly string actionId;

        public ActionStateRuleAttribute(string id, string actionId, Nesting targetViewNesting, string normalCriteria,
                                        string emptyCriteria, ViewType viewType, string module, ActionState actionState,
                                        string viewId)
            : base(id, targetViewNesting, normalCriteria, emptyCriteria, viewType, module, viewId) {
            this.actionId = actionId;
            ActionState = actionState;
        }


        public string ActionId {
            get { return actionId; }
        }
        #region IActionStateRule Members
        public ActionState ActionState { get; set; }
        

        string IActionStateRule.ActionId {
            get { return actionId; }
            set { throw new NotImplementedException(); }
        }
        #endregion
    }
}