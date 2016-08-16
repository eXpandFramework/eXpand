using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Fasterflect;
using Xpand.Persistent.Base.ModelDifference;
using Xpand.Persistent.BaseImpl.Security.PermissionPolicyData;

namespace Xpand.Persistent.BaseImpl.ModelDifference{
    [System.ComponentModel.DisplayName("ModelCombine")]
    public class ModelCombineOperationPermissionPolicyData : PermissionPolicyData, IModelCombinePermission{
        private string _difference;
        private ApplicationModelCombineModifier _modifier = ApplicationModelCombineModifier.Allow;

        public ModelCombineOperationPermissionPolicyData(Session session)
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
            var typeInfo = GetPermissionTypeInfo<IModelCombinePermission>();
            var modelCombinePermission =(IModelCombinePermission)typeInfo.Type.CreateInstance(_modifier, Difference+"");
            return new[]{(IOperationPermission) modelCombinePermission};
        }

        
    }
}