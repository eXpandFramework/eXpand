using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public class ActionStateRuleController : ConditionalLogicRuleViewController<IActionStateRule> {
        public override void ExecuteRule(LogicRuleInfo<IActionStateRule> logicRuleInfo, ExecutionContext executionContext) {
            IActionStateRule rule = logicRuleInfo.Rule;
            foreach (ActionBase actionBase in GetActions(rule)) {
                switch (rule.ActionState) {
                    case ActionState.Hidden:
                        ActivateDeActivateAction(logicRuleInfo, actionBase);
                        break;
                    case ActionState.Disabled:
                        EnableDisableAction(logicRuleInfo, actionBase);
                        break;
                    case ActionState.Executed: {
                        if (logicRuleInfo.Active) {
                            ExecuteAction(actionBase, executionContext);
                        }
                    }
                        break;
                    case ActionState.ExecutedAndDisable:
                        ExecuteAndDisableAction(actionBase, executionContext);
                        break;
                }
            }            
        }

        void ActivateDeActivateAction(LogicRuleInfo<IActionStateRule> info, ActionBase actionBase) {
            actionBase.Active[ActiveObjectTypeHasRules] = !info.Active;
        }

        void EnableDisableAction(LogicRuleInfo<IActionStateRule> info, ActionBase actionBase) {
            actionBase.Enabled[ActiveObjectTypeHasRules] = !info.Active;
        }

        void ExecuteAction(ActionBase actionBase, ExecutionContext executionContext) {
            if (executionContext==ExecutionContext.ViewControlsCreated) {
                var simpleAction = ((SimpleAction)actionBase);
                if (simpleAction.Active && simpleAction.Enabled){
                    simpleAction.DoExecute();
                }
            }
        }

        void ExecuteAndDisableAction(ActionBase actionBase, ExecutionContext executionContext) {
            if (executionContext==ExecutionContext.ViewControlAdding) {
                var simpleAction = ((SimpleAction)actionBase);
                simpleAction.Active[ActiveObjectTypeHasRules] = true;
                if (simpleAction.Active && simpleAction.Enabled)
                    simpleAction.DoExecute();
                simpleAction.Active[ActiveObjectTypeHasRules] = false;
            }
        }

        private IEnumerable<ActionBase> GetActions(IActionStateRule rule){
            IEnumerable<ActionBase> actionBases =
                Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions);
            if (!string.IsNullOrEmpty(rule.Module)){
                return GetModuleActions(rule, actionBases);
            }
            ActionBase actionBase = actionBases.Where(@base => @base.Id == rule.ActionId).Select(@base => @base).Single();
            return new List<ActionBase> { actionBase };
        }
        IEnumerable<ActionBase> GetModuleActions(IActionStateRule rule, IEnumerable<ActionBase> actionBases){
            IEnumerable<string> assemblies =
                Application.Modules.Where(@base => new Regex(rule.Module).IsMatch(@base.GetType().FullName)).Select(
                                                                                                                       @base => @base.GetType().Assembly.FullName);
            return actionBases.Where(@base => assemblies.Contains(@base.Controller.GetType().Assembly.FullName));
        }

    }
}