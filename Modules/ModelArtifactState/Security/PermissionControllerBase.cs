using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl;
using System.Linq;
using eXpand.ExpressApp.Security.Calculators;

namespace eXpand.ExpressApp.ModelArtifactState.Security
{
    public abstract class PermissionControllerBase<StatePermission> : ViewController where StatePermission : ExpressApp.Security.Permissions.StatePermission
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            activateAllControllersForAllPermissionContexts();
            if (SecuritySystem.Instance is ISecurityComplex)
            {
                var objectTypePermissions = getObjectTypePermissions();
                var viewPermissions = getViewPermissions(objectTypePermissions);
                var listViewPermissions = getListViewPermissions(viewPermissions);
                var detailViewPermissions = getDetailViewPermissions(viewPermissions);
                SetArtifactState(listViewPermissions,criteria =>View.CurrentObject== null? criteria.EmptyCriteria:criteria.NormalCriteria,View.CurrentObject??new object());
                SetArtifactState(detailViewPermissions, criteria => criteria.NormalCriteria, View.CurrentObject);
            }
            else
                Active["NoComplexSecurity"] = false;
        }

        private void activateAllControllersForAllPermissionContexts()
        {
            foreach (var controller in Frame.Controllers.Values)
            {
                IEnumerable<string> keys = controller.Active.GetKeys().Where(s => s.StartsWith(ActivationContext));
                foreach (var key in keys)
                    controller.Active[key] = true;
            }
        }

        protected abstract string ActivationContext { get; }

        private IEnumerable<StatePermission> getViewPermissions(IEnumerable<StatePermission> objectTypePermissions)
        {
            return objectTypePermissions.Where(
                permission => View.Id == permission.View || string.IsNullOrEmpty(permission.View));
        }


        private delegate string criteriaDelegate(StatePermission criteria);
        private void SetArtifactState(IEnumerable<StatePermission> permissions, criteriaDelegate criteriaDelegate, object currentObject)
        {
            foreach (var permission in permissions)
            {
                var activation = StateCalculator.ComputeControllerActivation(currentObject,criteriaDelegate.Invoke(permission));
                if (!activation)
                    DeactivateArtifact(permission);
            }
        }

        protected abstract void DeactivateArtifact(StatePermission statePermission);


        private IEnumerable<StatePermission> getDetailViewPermissions(IEnumerable<StatePermission> objectTypePermissions)
        {
            return objectTypePermissions.Where(permission =>StateCalculator.IsDetailViewTargeted(View, permission));
        }

        private IEnumerable<StatePermission> getListViewPermissions(IEnumerable<StatePermission> objectTypePermissions)
        {
            return objectTypePermissions.Where(permission =>StateCalculator.IsListViewTargeted(permission, View));
        }

        private IEnumerable<StatePermission> getObjectTypePermissions()
        {
            return
                ((User) SecuritySystem.CurrentUser).Permissions.OfType<StatePermission>().Where(
                    permission => permission.ObjectType == null || permission.ObjectType == View.ObjectTypeInfo.Type);
        }
    }
}