using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Interfaces;
using eXpand.ExpressApp.Security.Attributes;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes
{
    public class StateModelArtifactRuleAttribute:StateRuleAttribute,IArtifactStateRule
    {
        public StateModelArtifactRuleAttribute(string id, Nesting targetViewNesting, string normalCriteria, string emptyCriteria,
                                               ViewType viewType, string module,State state,string viewId)
            : base(id, targetViewNesting, normalCriteria, emptyCriteria, viewType,state,viewId)
        {
            Module = module;
        }


        public string Module { get; set; }

    }
}