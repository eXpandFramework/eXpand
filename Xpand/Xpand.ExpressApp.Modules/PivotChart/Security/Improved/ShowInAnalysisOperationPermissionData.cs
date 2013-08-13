using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Security.Improved;
using Xpand.ExpressApp.PivotChart.ShowInAnalysis;

namespace Xpand.ExpressApp.PivotChart.Security.Improved {
    [System.ComponentModel.DisplayName("ShowInAnalysis")]
    public class ShowInAnalysisOperationPermissionData : ControllerStateOperationPermissionData {
        public ShowInAnalysisOperationPermissionData(Session session)
            : base(session) {
            ControllerType = typeof(ShowInAnalysisViewController);
            NormalCriteria = "1=1";
        }
        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new ShowInAnalysisPermission(this) };
        }

    }
}