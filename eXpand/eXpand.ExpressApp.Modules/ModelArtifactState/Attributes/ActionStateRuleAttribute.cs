using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes
{
    public class ActionStateRuleAttribute:ArtifactStateRuleAttribute,IActionStateRule
    {
        private readonly string actionId;

        public ActionStateRuleAttribute(string id,string actionId, Nesting targetViewNesting, string normalCriteria,
                                        string emptyCriteria, ViewType viewType, string module, State state, string viewId)
            : base(id, targetViewNesting, normalCriteria, emptyCriteria, viewType, module, state,viewId)
        {
            this.actionId = actionId;
        }

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