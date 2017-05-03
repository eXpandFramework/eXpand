using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelArtifact;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic {
    public class ControllerStateRuleController : ViewController {
        public const string ActiveObjectTypeHasControllerRules="ActiveObjectTypeHasControllerRules";
        protected void ChangeState(LogicRuleInfo info) {
            var rule = ((IControllerStateRule)info.Rule);
            Frame.GetController(rule.ControllerType, controller => {
                if (rule.ControllerState == Persistent.Base.ModelArtifact.ControllerState.Register) {
                    if (!controller.Active) {
                        Frame.RegisterController(controller);
                    }
                }
                else {
                    controller.Active[ActiveObjectTypeHasControllerRules] = info.InvertCustomization || ((IControllerStateRule)info.Rule).ControllerState == Persistent.Base.ModelArtifact.ControllerState.Enabled;
                }
            });
        }

        void ChangeStateOnModules(LogicRuleInfo info) {
            var controllerStateRule = ((IControllerStateRule)info.Rule);
            IEnumerable<string> assemblies = GetAssemblies(controllerStateRule);
            var controllers = GetControllers(assemblies);
            foreach (Controller controller in controllers)
                controller.Active[ActiveObjectTypeHasControllerRules] = !info.Active;
        }

        IEnumerable<string> GetAssemblies(IControllerStateRule controllerStateRule) {
            return Application.Modules.Where(@base => new Regex(controllerStateRule.Module).IsMatch(@base.GetType().FullName + "")).
                Select(@base => @base.GetType().Assembly.FullName);
        }

        IEnumerable<Controller> GetControllers(IEnumerable<string> assemblies) {
            return Frame.Controllers.Cast<Controller>().Where(controller => assemblies.Contains(controller.GetType().Assembly.FullName));
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            Frame.GetController<LogicRuleViewController>(controller => controller.LogicRuleExecutor.LogicRuleExecute += LogicRuleExecutorOnLogicRuleExecute);
            Frame.Disposing += FrameOnDisposing;
        }

        private void FrameOnDisposing(object sender, EventArgs e){
            Frame.Disposing -= FrameOnDisposing;
            Frame.GetController<LogicRuleViewController>(controller => controller.LogicRuleExecutor.LogicRuleExecute -= LogicRuleExecutorOnLogicRuleExecute);
        }

        private void LogicRuleExecutorOnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs e){
            var info = e.LogicRuleInfo;
            var controllerStateRule = info.Rule as IControllerStateRule;
            if (controllerStateRule != null) {
                if (!string.IsNullOrEmpty(controllerStateRule.Module)) {
                    ChangeStateOnModules(info);
                }
                else if (info.Active)
                    ChangeState(info);
            }
        }

    }
}


