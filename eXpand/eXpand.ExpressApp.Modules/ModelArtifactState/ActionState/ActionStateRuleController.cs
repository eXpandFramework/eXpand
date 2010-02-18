using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.ModelArtifactState.ActionState {
    public class ActionStateRuleController : ModelRuleViewController<ActionStateRuleAttribute,ActionStateRuleNodeWrapper,ActionStateRuleInfo,ActionStateRule,ActionStateRulePermission> {
        public override void ExecuteRule(ActionStateRuleInfo info, ExecutionReason executionReason) {
            ActionStateRule rule = info.Rule;
            foreach (ActionBase actionBase in GetActions(rule)) {
                switch (rule.ActionState) {
                    case ActionState.Hidden:
                        ActivateDeActivatection(info, actionBase);
                        break;
                    case ActionState.Disabled:
                        EnableDisableAction(info, actionBase);
                        break;
                    case ActionState.Executed:
                        ExecuteAction(actionBase, executionReason);
                        break;
                    case ActionState.ExecutedAndDisable:
                        ExecuteAndDisableAction(actionBase, executionReason);
                        break;
                }
            }            
        }

        void ActivateDeActivatection(ActionStateRuleInfo info, ActionBase actionBase) {
            actionBase.Active[ActiveObjectTypeHasRules] = !info.Active;
        }

        void EnableDisableAction(ActionStateRuleInfo info, ActionBase actionBase) {
            actionBase.Enabled[ActiveObjectTypeHasRules] = !info.Active;
        }

        void ExecuteAction(ActionBase actionBase, ExecutionReason executionReason) {
            if (executionReason==ExecutionReason.ViewControlsCreated) {
                var simpleAction = ((SimpleAction)actionBase);
                if (simpleAction.Active&&simpleAction.Enabled)
                    simpleAction.DoExecute();
            }
        }

        void ExecuteAndDisableAction(ActionBase actionBase, ExecutionReason executionReason) {
            if (executionReason==ExecutionReason.ViewControlsCreated) {
                var simpleAction = ((SimpleAction)actionBase);
                simpleAction.Active[ActiveObjectTypeHasRules] = true;
                if (simpleAction.Active && simpleAction.Enabled)
                    simpleAction.DoExecute();
                simpleAction.Active[ActiveObjectTypeHasRules] = false;
            }
        }

        private IEnumerable<ActionBase> GetActions(ActionStateRule rule){
            IEnumerable<ActionBase> actionBases =
                Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions);
            if (!string.IsNullOrEmpty(rule.Module)){
                return GetModuleActions(rule, actionBases);
            }
            ActionBase actionBase = actionBases.Where(@base => @base.Id == rule.ActionId).Select(@base => @base).Single();
            return new List<ActionBase> { actionBase };
        }
        IEnumerable<ActionBase> GetModuleActions(ActionStateRule rule, IEnumerable<ActionBase> actionBases){
            IEnumerable<string> assemblies =
                Application.Modules.Where(@base => new Regex(rule.Module).IsMatch(@base.GetType().FullName)).Select(
                    @base => @base.GetType().Assembly.FullName);
            return actionBases.Where(@base => assemblies.Contains(@base.Controller.GetType().Assembly.FullName));
        }

    }
}