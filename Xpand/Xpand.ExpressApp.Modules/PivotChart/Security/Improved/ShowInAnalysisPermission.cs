using System.Collections.Generic;
using Xpand.ExpressApp.ConditionalControllerState.Logic;
using Xpand.ExpressApp.ConditionalControllerState.Security.Improved;

namespace Xpand.ExpressApp.PivotChart.Security.Improved {
    public class ShowInAnalysisPermission : ControllerStateRulePermission {
        public new const string OperationName = "ShowInAnalysis";
        public ShowInAnalysisPermission(IControllerStateRule logicRule)
            : base(OperationName, logicRule) {
        }
        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }

    }
}