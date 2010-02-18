using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ControllerState{
    public class ControllerStateRuleController : ModelRuleViewController<ControllerStateRuleAttribute, ControllerStateRuleNodeWrapper, ControllerStateRuleInfo, ControllerStateRule, ControllerStateRulePermission>{
        public override void ExecuteRule(ControllerStateRuleInfo info, ExecutionReason executionReason) {
            if (!string.IsNullOrEmpty(info.Rule.Module)){
                ChangeStateOnModules(info);
            }
            else
                ChangeState(info);
        }

        void ChangeState(ControllerStateRuleInfo info) {
            Frame.GetController(info.Rule.ControllerType).Active[ActiveObjectTypeHasRules] = !info.Active;
        }

        void ChangeStateOnModules(ControllerStateRuleInfo info) {
            var controllerStateRule = info.Rule;
            IEnumerable<string> assemblies =GetAssemblies(controllerStateRule);
            var controllers = GetControllers(assemblies);
            foreach (Controller controller in controllers)
                controller.Active[ActiveObjectTypeHasRules] =!info.Active;
        }

        IEnumerable<string> GetAssemblies(ControllerStateRule controllerStateRule) {
            return Application.Modules.Where(@base =>new Regex(controllerStateRule.Module).IsMatch(@base.GetType().FullName)).
                Select(@base => @base.GetType().Assembly.FullName);
        }

        IEnumerable<Controller> GetControllers(IEnumerable<string> assemblies) {
            return Frame.Controllers.Cast<Controller>().Where(controller => assemblies.Contains(controller.GetType().Assembly.FullName));
        }
    }
}
