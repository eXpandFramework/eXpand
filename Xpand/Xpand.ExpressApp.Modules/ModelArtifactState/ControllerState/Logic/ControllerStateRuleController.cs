using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic {
    public class ControllerStateRuleController : ViewController {
        LogicRuleViewController _logicRuleViewController;
        public const string ActiveObjectTypeHasControllerRules="ActiveObjectTypeHasControllerRules";
        protected void ChangeState(LogicRuleInfo info) {
            var rule = ((IControllerStateRule)info.Rule);
            Controller controller = Frame.GetController(rule.ControllerType);
            if (rule.ControllerState == ControllerState.Register) {
                if (!controller.Active) {
                    Frame.RegisterController(controller);
                }
            }
            else{
                controller.Active[ActiveObjectTypeHasControllerRules] = info.InvertCustomization || ((IControllerStateRule)info.Rule).ControllerState == ControllerState.Enabled;
            }
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
            _logicRuleViewController = Frame.GetController<LogicRuleViewController>();
            Frame.Disposing += FrameOnDisposing;
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute += LogicRuleExecutorOnLogicRuleExecute;
        }

        private void FrameOnDisposing(object sender, EventArgs e){
            Frame.Disposing -= FrameOnDisposing;
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute -= LogicRuleExecutorOnLogicRuleExecute;
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


