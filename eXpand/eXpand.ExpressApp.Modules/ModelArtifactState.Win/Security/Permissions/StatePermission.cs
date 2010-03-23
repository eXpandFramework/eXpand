using eXpand.ExpressApp.Security.Permissions;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.ModelArtifactState.Security.Permissions {
    public abstract class StatePermission:RulePermission {
        public State State { get; set; }
    }
}