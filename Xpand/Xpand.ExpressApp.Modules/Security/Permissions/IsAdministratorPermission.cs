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

    public class IsAdministratorPermissionRequestProcessor : PermissionRequestProcessorBase<IsAdministratorPermissionRequest>,ICustomPermissionRequestProccesor {
        


        public override bool IsGranted(IsAdministratorPermissionRequest permissionRequest) {
            return (Permissions.FindFirst<IsAdministratorPermission>() != null);
        }

        public IPermissionDictionary Permissions{ get; set; }
    }
}