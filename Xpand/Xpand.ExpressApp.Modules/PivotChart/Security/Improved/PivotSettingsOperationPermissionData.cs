using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Xpand.ExpressApp.ConditionalControllerState.Security.Improved;

namespace Xpand.ExpressApp.PivotChart.Security.Improved {
    public class PivotSettingsOperationPermissionData : ControllerStateOperationPermissionData {
        public PivotSettingsOperationPermissionData(Session session)
            : base(session) {
            ControllerType = typeof(PivotOptionsController);
            NormalCriteria = "1=1";
        }
        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new PivotSettingsPermission(this) };
        }

        protected override string GetPermissionInfoCaption() {
            return string.Format("{1}: {0}", ID, GetType().Name);
        }
    }
}