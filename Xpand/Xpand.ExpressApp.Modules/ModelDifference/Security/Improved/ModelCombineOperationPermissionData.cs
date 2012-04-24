using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.ExpressApp.ModelDifference.Security.Improved {
    public class ModelCombineOperationPermissionData : XpandPermissionData, IModelCombinePermission {

        private ApplicationModelCombineModifier modifier = ApplicationModelCombineModifier.Allow;

        public ModelCombineOperationPermissionData(Session session) : base(session) {
        }

        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new ModelCombOperationinePermission(modifier,Difference) };
        }
        protected override string GetPermissionInfoCaption() {
            var enumDescriptor = new EnumDescriptor(typeof(ApplicationModelCombineModifier));
            return CaptionHelper.GetClassCaption(GetType().FullName) + " (" + enumDescriptor.GetCaption(Modifier) + ")";
        }
        public ApplicationModelCombineModifier Modifier {
            get { return modifier; }
            set { modifier = value; }
        }
        private string _difference;
        public string Difference {
            get {
                return _difference;
            }
            set {
                SetPropertyValue("Difference", ref _difference, value);
            }
        }
    }
}