using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Base.Security;

namespace Xpand.ExpressApp.Validation.Controllers {
    public class PermissionValidationController : ViewController {
        private PersistenceValidationController _persistenceValidationController;

        public PermissionValidationController() {
            TargetObjectType = typeof(IPersistentPermission);
            TargetViewType = ViewType.DetailView;
        }

        private void controller_ContextValidating(object sender, ContextValidatingEventArgs e) {
            IPermission permission = ((IPersistentPermission)View.CurrentObject).Permission;
            if (!e.TargetObjects.Contains(permission))
                e.TargetObjects.Add(permission);
        }

        protected override void OnActivated() {
            base.OnActivated();
            _persistenceValidationController = Frame.GetController<PersistenceValidationController>();
            if (_persistenceValidationController != null)
                _persistenceValidationController.ContextValidating += controller_ContextValidating;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_persistenceValidationController != null) {
                _persistenceValidationController.ContextValidating -= controller_ContextValidating;
                _persistenceValidationController = null;
            }
        }
    }
}