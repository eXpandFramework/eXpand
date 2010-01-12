using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.Attributes {
    public class NotAllowObjectInstanciationRule : ControllerStateRuleAttribute {
        public NotAllowObjectInstanciationRule(string id, Nesting targetViewNesting, string normalCriteria,
                                               string emptyCriteria, ViewType viewType, State state, string viewId)
            : base(
                id, typeof(NewObjectViewController), targetViewNesting, normalCriteria, emptyCriteria, viewType, "",
                state, viewId) {
        }

        public NotAllowObjectInstanciationRule()
            : this(typeof(NewObjectViewController).Name, Nesting.Any, "1=1", "1=1", ViewType.Any, State.Disabled, null)
        {
        }
    }
}