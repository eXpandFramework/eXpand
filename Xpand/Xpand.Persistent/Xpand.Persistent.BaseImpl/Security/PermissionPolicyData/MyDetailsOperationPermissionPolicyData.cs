using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.Persistent.BaseImpl.Security.PermissionPolicyData{
    [System.ComponentModel.DisplayName(MyDetailsPermission.OperationName)]
    public class MyDetailsOperationPermissionPolicyData : ModifierPermissionPolicyData{
        public MyDetailsOperationPermissionPolicyData(Session session)
            : base(session){
        }

        public override IList<IOperationPermission> GetPermissions(){
            return new IOperationPermission[]{new MyDetailsPermission(Modifier)};
        }

    }
}