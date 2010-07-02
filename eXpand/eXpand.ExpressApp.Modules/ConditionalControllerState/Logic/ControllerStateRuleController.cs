using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Conditional.Logic;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.ConditionalControllerState.Logic
{
    public class ControllerStateRuleController : ConditionalLogicRuleViewController<IControllerStateRule>{
        protected void ChangeState(LogicRuleInfo<IControllerStateRule> info){
            Frame.GetController(info.Rule.ControllerType).Active[ActiveObjectTypeHasRules] = !info.Active;
        }

        void ChangeStateOnModules(LogicRuleInfo<IControllerStateRule> info){
            var controllerStateRule = info.Rule;
            IEnumerable<string> assemblies = GetAssemblies(controllerStateRule);
            var controllers = GetControllers(assemblies);
            foreach (Controller controller in controllers)
                controller.Active[ActiveObjectTypeHasRules] = !info.Active;
        }

        IEnumerable<string> GetAssemblies(IControllerStateRule controllerStateRule) {
                return Application.Modules.Where(@base => new Regex(controllerStateRule.Module).IsMatch(@base.GetType().FullName+"")).
                    Select(@base => @base.GetType().Assembly.FullName);
        }

        IEnumerable<Controller> GetControllers(IEnumerable<string> assemblies){
            return Frame.Controllers.Cast<Controller>().Where(controller => assemblies.Contains(controller.GetType().Assembly.FullName));
        }

        public override void ExecuteRule(LogicRuleInfo<IControllerStateRule> info, ExecutionContext executionContext){
            if (!string.IsNullOrEmpty(info.Rule.Module)){
                ChangeStateOnModules(info);
            }
            else
                ChangeState(info);
        }

        protected override IModelGroupContexts GetModelGroupContexts(string executionContextGroup) {
            return ((IModelApplicationModelArtifactState)Application.Model).ModelArtifactState.ConditionalControllerState.GroupContexts;
        }
    }
}


