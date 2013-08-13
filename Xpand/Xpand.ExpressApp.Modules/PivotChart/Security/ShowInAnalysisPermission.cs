using DevExpress.Xpo;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Security;
using Xpand.ExpressApp.PivotChart.ShowInAnalysis;

namespace Xpand.ExpressApp.PivotChart.Security {
    [NonPersistent]
    public class ShowInAnalysisPermission : ControllerStateRulePermission {
        public ShowInAnalysisPermission() {
            ControllerType = typeof (ShowInAnalysisViewController);
            NormalCriteria = "1=1";
        }

        public override string ToString() {
            return string.Format("{1}: {0}", ID, GetType().Name);
        }
    }
}