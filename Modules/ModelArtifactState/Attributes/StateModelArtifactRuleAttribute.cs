using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.Security.Attributes;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes
{
    public class StateModelArtifactRuleAttribute:StateRuleAttribute,IModuleRule
    {
        public StateModelArtifactRuleAttribute(Nesting targetViewNesting, string normalCriteria, string emptyCriteria, ViewType viewType, string module) : base(targetViewNesting, normalCriteria, emptyCriteria, viewType, module)
        {
        }

        public string Module { get; set; }
    }
}