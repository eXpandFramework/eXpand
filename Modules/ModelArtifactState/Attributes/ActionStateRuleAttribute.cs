using DevExpress.ExpressApp;
using eXpand.ExpressApp.Security.Attributes;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes
{
    public class ActionStateRuleAttribute:StateRuleAttribute
    {
        public ActionStateRuleAttribute(string actionId, Nesting TargetViewNesting, string NormalCriteria,
                                             string EmptyCriteria, ViewType viewType, string module)
            : base(TargetViewNesting, NormalCriteria, EmptyCriteria, viewType, module)
        {
            ActionId = actionId;
        }

        public string ActionId { get; set; }
    }
}