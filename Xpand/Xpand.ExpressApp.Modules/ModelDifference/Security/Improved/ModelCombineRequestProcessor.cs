using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.ExpressApp.ModelDifference.Security.Improved {
    public class ModelCombineRequestProcessor : PermissionRequestProcessorBase<ModelCombinePermissionRequest> ,ICustomPermissionRequestProccesor{
       
        public override bool IsGranted(ModelCombinePermissionRequest permissionRequest) {
            var modelCombinePermission = Permissions.FindFirst<ModelCombineOperationPermission>();
            return modelCombinePermission != null && permissionRequest.Modifier == modelCombinePermission.Modifier;
        }

        public IPermissionDictionary Permissions{ get; set; }
    }
}