using System;
using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.ModelDifference.Security.Improved {
    [Serializable]
    public class ModelCombinePermissionRequest : OperationPermissionRequestBase {
        public ModelCombinePermissionRequest(ApplicationModelCombineModifier modifier)
            : base(ModelCombinePermission.OperationName) {
            Modifier = modifier;

        }
        public ApplicationModelCombineModifier Modifier { get; private set; }


    }
}