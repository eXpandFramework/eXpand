using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.ModelDifference.Security.Improved {
    public class ModelCombineRequestProcessor : PermissionRequestProcessorBase<ModelCombinePermissionRequest> {
        readonly IPermissionDictionary _permissions;

        public ModelCombineRequestProcessor(IPermissionDictionary permissions) {
            _permissions = permissions;
        }

        public override bool IsGranted(ModelCombinePermissionRequest permissionRequest) {
            var modelCombinePermission = _permissions.FindFirst<ModelCombineOperationPermission>();
            return modelCombinePermission != null && permissionRequest.Modifier == modelCombinePermission.Modifier;
        }

    }
}