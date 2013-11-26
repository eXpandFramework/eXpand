using System;
using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.Security.Permissions {
    [Serializable]
    public class IsAdministratorPermissionRequest : IPermissionRequest {
        public object GetHashObject() {
            return OperationPermissionRequestBase.UseStringHashCodeObject
                       ? (object) typeof (IsAdministratorPermissionRequest).FullName
                       : typeof (IsAdministratorPermissionRequest).GetHashCode();
        }
    }

    public class IsAdministratorPermissionRequestProcessor : PermissionRequestProcessorBase<IsAdministratorPermissionRequest> {
        private readonly IPermissionDictionary permissions;

        public IsAdministratorPermissionRequestProcessor(IPermissionDictionary permissions) {
            this.permissions = permissions;
        }

        public override bool IsGranted(IsAdministratorPermissionRequest permissionRequest) {
            return (permissions.FindFirst<IsAdministratorPermission>() != null);
        }
    }
}