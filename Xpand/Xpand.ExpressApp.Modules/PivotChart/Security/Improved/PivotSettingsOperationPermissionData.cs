using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Security.Improved;

namespace Xpand.ExpressApp.PivotChart.Security.Improved {
    [System.ComponentModel.DisplayName("PivotSettings")]
    public class PivotSettingsOperationPermissionData : ControllerStateOperationPermissionData {
        public PivotSettingsOperationPermissionData(Session session)
            : base(session) {
            ControllerType = typeof(PivotOptionsController);
            NormalCriteria = "1=1";
        }
        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new PivotSettingsPermission(this) };
        }

    }
}