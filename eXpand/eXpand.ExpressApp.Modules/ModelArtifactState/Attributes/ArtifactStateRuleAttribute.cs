using DevExpress.ExpressApp;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes
{
    public abstract class ArtifactStateRuleAttribute : StateModelArtifactRuleAttribute
    {
        protected ArtifactStateRuleAttribute(string id,Nesting targetViewNesting, string normalCriteria, string emptyCriteria,
                                             ViewType viewType, string module, State state, string viewId)
            : base(id, targetViewNesting, normalCriteria, emptyCriteria, viewType, module,state,viewId)
        {
        }

        
    }
}