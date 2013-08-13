using DevExpress.Xpo;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Security;

namespace Xpand.ExpressApp.PivotChart.Security {
    [NonPersistent]
    public class PivotSettingsPermission : ControllerStateRulePermission {
        public PivotSettingsPermission() {
            ControllerType = typeof (PivotOptionsController);
            NormalCriteria = "1=1";
        }

        public override string ToString() {
            return string.Format("{1}: {0}", ID, GetType().Name);
        }
    }
}