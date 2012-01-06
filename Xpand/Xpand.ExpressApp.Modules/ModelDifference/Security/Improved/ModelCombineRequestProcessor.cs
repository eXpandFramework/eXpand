using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.ModelDifference.Security.Improved {
    public class ModelCombineRequestProcessor : PermissionRequestProcessorBase<ModelCombinePermissionRequest> {
        protected override bool IsRequestFit(ModelCombinePermissionRequest permissionRequest, OperationPermissionBase permission, IRequestSecurityStrategy securityInstance) {
            if (permission is ModelCombinePermission) {
                return  permissionRequest.Modifier == ((ModelCombinePermission)permission).Modifier;
            }
            return false;
        }
    }
}