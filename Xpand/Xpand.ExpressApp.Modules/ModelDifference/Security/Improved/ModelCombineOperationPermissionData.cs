using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.ExpressApp.ModelDifference.Security.Improved{
    [System.ComponentModel.DisplayName("ModelCombine")]
    public class ModelCombineOperationPermissionData : XpandPermissionData, IModelCombinePermission{
        private string _difference;
        private ApplicationModelCombineModifier _modifier = ApplicationModelCombineModifier.Allow;

        public ModelCombineOperationPermissionData(Session session)
            : base(session){
        }

        public ApplicationModelCombineModifier Modifier{
            get { return _modifier; }
            set { _modifier = value; }
        }

        public string Difference{
            get { return _difference; }
            set { SetPropertyValue("Difference", ref _difference, value); }
        }

        public override IList<IOperationPermission> GetPermissions(){
            return new IOperationPermission[]{new ModelCombineOperationPermission(_modifier, Difference)};
        }
    }
}