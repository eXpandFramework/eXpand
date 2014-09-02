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
        private readonly IPermissionDictionary _permissions;

        public IsAdministratorPermissionRequestProcessor(IPermissionDictionary permissions) {
            _permissions = permissions;
        }

        public override bool IsGranted(IsAdministratorPermissionRequest permissionRequest) {
            return (_permissions.FindFirst<IsAdministratorPermission>() != null);
        }
    }
}