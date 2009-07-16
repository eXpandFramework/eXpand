using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.ModelArtifactState.Providers;
using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.Security.Actions
{
    public partial class PermissionsForActionsController : PermissionControllerBase<ActionsStatePermission>
    {
        public PermissionsForActionsController()
        {
            InitializeComponent();
            RegisterActions((IContainer) components);
        }

        protected override string ActivationContext
        {
            get { return eXpand.ExpressApp.ModelArtifactState.ActivationContext.ActionsStatePermissionActivation; }
        }

        protected override void DeactivateArtifact(ActionsStatePermission statePermission)
        {
            var provider = new ActionProvider(statePermission, this, statePermission.ActionId);
            ICollection<ActionBase> actionBases=provider.Find();
            foreach (var actionBase in actionBases)
                actionBase.Active[string.Format("{0} {1}", ActivationContext, statePermission.Name)]
                    = false;

        }



    }
}