using System;


namespace Xpand.ExpressApp.ModelDifference.Security.Improved {
    [Serializable]
    public class ModelCombinePermissionRequest : DevExpress.ExpressApp.Security.OperationPermissionRequestBase {
        public ModelCombinePermissionRequest(ApplicationModelCombineModifier modifier)
            : base(ModelCombOperationinePermission.OperationName) {
            Modifier = modifier;
        }

        public ApplicationModelCombineModifier Modifier { get; private set; }


        public override object GetHashObject() {
            return "ModelCombinePermissionRequest";
        }
    }
}