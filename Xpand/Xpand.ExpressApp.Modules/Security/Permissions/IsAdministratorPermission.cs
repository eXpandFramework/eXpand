using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.Security.Permissions {
    public class IsAdministratorPermissionRequest : IPermissionRequest {
        public object GetHashObject() {
            return GetType().FullName;
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