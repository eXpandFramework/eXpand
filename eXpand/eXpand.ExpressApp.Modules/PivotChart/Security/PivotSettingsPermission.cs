using DevExpress.Xpo;
using eXpand.ExpressApp.ConditionalControllerState.Security;

namespace eXpand.ExpressApp.PivotChart.Security {
    [NonPersistent]
    public class PivotSettingsPermission : ControllerStateRulePermission {
        public PivotSettingsPermission() {
            ControllerType = typeof (PivotOptionsController).FullName;
            NormalCriteria = "1=1";
        }

        public override string ToString() {
            return string.Format("{1}: {0}", ID, GetType().Name);
        }
    }
}