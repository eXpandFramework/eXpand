using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.ModelArtifactState.ControllerState.Logic;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes {
    public class NotAllowObjectInstanciationRuleAttribute : ControllerStateRuleAttribute {
        public NotAllowObjectInstanciationRuleAttribute(string id, Nesting targetViewNesting, string normalCriteria,
                                               string emptyCriteria, ViewType viewType, State state, string viewId)
            : base(
                id, typeof(NewObjectViewController), targetViewNesting, normalCriteria, emptyCriteria, viewType, "",
                state, viewId) {
        }

        public NotAllowObjectInstanciationRuleAttribute()
            : this(typeof(NewObjectViewController).Name, Nesting.Any, "1=1", "1=1", ViewType.Any, State.Disabled, null)
        {
        }
    }
}