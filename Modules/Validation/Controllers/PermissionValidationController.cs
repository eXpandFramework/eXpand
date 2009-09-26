using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base.Security;

namespace eXpand.ExpressApp.Validation.Controllers
{
    public partial class PermissionValidationController : ViewController
    {
        private PersistenceValidationController persistenceValidationController;

        public PermissionValidationController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(IPersistentPermission);
            TargetViewType = ViewType.DetailView;
        }

        private void controller_ContextValidating(object sender, ContextValidatingEventArgs e)
        {
            IPermission permission = ((IPersistentPermission)View.CurrentObject).Permission;
            if (!e.TargetObjects.Contains(permission))
                e.TargetObjects.Add(permission);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            persistenceValidationController = Frame.GetController<PersistenceValidationController>();
            if (persistenceValidationController != null)
                persistenceValidationController.ContextValidating += controller_ContextValidating;
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            if (persistenceValidationController != null)
            {
                persistenceValidationController.ContextValidating -= controller_ContextValidating;
                persistenceValidationController = null;
            }
        }
    }
}