using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState
{
    public class ActivationContext
    {
        public static readonly string ViewsControllerActivation = "ViewsControllerActivation";
        public static readonly string ViewsActionActivation = "ViewsActionActivation";
        public static readonly string DeclarativeControllerActivation = "DeclarativeControllerActivation";
        public static readonly string DeclarativeActionActivation = "DeclarativeActionActivation";
        public static readonly string ControllersStatePermissionActivation = typeof(ControllersStatePermission).Name;
        public static readonly string ActionsStatePermissionActivation = typeof(ActionsStatePermission).Name;
    }
}