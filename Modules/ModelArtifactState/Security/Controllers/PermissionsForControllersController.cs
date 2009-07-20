using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelArtifactState.Providers;
using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.Security.Controllers
{
    public partial class PermissionsForControllersController : PermissionControllerBase<ControllersStatePermission>
    {
        public PermissionsForControllersController()
        {
            InitializeComponent();
            RegisterActions(components);
        }


        protected override string ActivationContext
        {
            get { return ModelArtifactState.ActivationContext.ControllersStatePermissionActivation; }
        }

        protected override void DeactivateArtifact(ControllersStatePermission statePermission)
        {
            var provider = new ControllerProvider(statePermission, this, statePermission.ControllerType);
            foreach (var controller in provider.Find())
                deActivate(statePermission, controller);
        }



        private void deActivate(ControllersStatePermission statePermission, Controller controller)
        {
            if (controller != null)
                controller.Active[string.Format("{0} {1}", ActivationContext, statePermission.Name)] = false;
        }


    }
}