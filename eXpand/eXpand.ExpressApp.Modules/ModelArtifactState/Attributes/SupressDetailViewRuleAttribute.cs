using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.ModelArtifactState.ControllerState;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes {
    public class SupressDetailViewRuleAttribute:ControllerStateRuleAttribute {
        public SupressDetailViewRuleAttribute(string id, Nesting targetViewNesting, string normalCriteria, string emptyCriteria,
                                              ViewType viewType, string module, State state, string viewId)
            : base(id, typeof(ListViewProcessCurrentObjectController), targetViewNesting, normalCriteria, emptyCriteria, viewType, module, state, viewId) {
        }
        public SupressDetailViewRuleAttribute()
            : this(typeof(ListViewProcessCurrentObjectController).Name,Nesting.Any, "1=1","1=1",ViewType.Any, "",State.Disabled,null)
        {
            
        }
    }
}