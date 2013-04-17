using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;

namespace Xpand.ExpressApp.ModelDifference.Security.Improved {
    public class ModelCombineOperationPermission : OperationPermissionBase, IModelCombinePermission {
        public const string OperationName = "ModelCombine";

        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
        public ModelCombineOperationPermission(ApplicationModelCombineModifier modifier, string difference)
            : base(OperationName) {
            Modifier = modifier;
            Difference = difference;
        }

        public override string ToString() {
            var enumDescriptor = new EnumDescriptor(typeof(ApplicationModelCombineModifier));
            return CaptionHelper.GetClassCaption(GetType().FullName) + " (" + enumDescriptor.GetCaption(Modifier) + "," + Difference + ")";
        }

        public ApplicationModelCombineModifier Modifier { get; set; }

        public string Difference { get; set; }
    }
}