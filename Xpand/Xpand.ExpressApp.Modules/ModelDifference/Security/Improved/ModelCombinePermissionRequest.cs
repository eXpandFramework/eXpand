using System;


namespace Xpand.ExpressApp.ModelDifference.Security.Improved {
    [Serializable]
    public class ModelCombinePermissionRequest : DevExpress.ExpressApp.Security.OperationPermissionRequestBase {
        public ModelCombinePermissionRequest(ApplicationModelCombineModifier modifier)
            : base(ModelCombinePermission.OperationName) {
            Modifier = modifier;
        }

        public ApplicationModelCombineModifier Modifier { get; private set; }


        public override object GetHashObject() {
            return typeof(ModelCombinePermissionRequest).GetHashCode();
        }
    }
}