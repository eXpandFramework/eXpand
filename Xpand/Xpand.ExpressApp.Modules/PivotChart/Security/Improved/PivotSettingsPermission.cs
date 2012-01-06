using System.Collections.Generic;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using Xpand.ExpressApp.ConditionalControllerState.Security.Improved;

namespace Xpand.ExpressApp.PivotChart.Security.Improved {
    public class PivotSettingsPermission : ControllerStateRulePermission {
        public new const string OperationName = "PivotSettings";
        public PivotSettingsPermission(IControllerStateRule logicRule)
            : base(OperationName, logicRule) {

        }

        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
    }
}
