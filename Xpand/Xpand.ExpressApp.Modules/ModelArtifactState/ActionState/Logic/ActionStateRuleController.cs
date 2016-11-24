using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Logic {
    public class ActionStateRuleController:ViewController {
        LogicRuleViewController _logicRuleViewController;
        public const string ActiveObjectTypeHasActionRules = "ActiveObjectTypeHasActionRules";

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.Disposing+=FrameOnDisposing;
            _logicRuleViewController = Frame.GetController<LogicRuleViewController>();
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute+=OnLogicRuleExecute;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing-=FrameOnDisposing;
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute-=OnLogicRuleExecute;
        }

        IEnumerable<ActionBase> GetActions(IActionStateRule rule) {
            var actionBases = Frame.Actions();
            if (!string.IsNullOrEmpty(rule.Module))
                return GetModuleActions(rule, actionBases);
            var contextIds = GetActionContextIds(((ActionStateRule) rule).ActionContext);
            var ids = new[]{rule.ActionId}.Concat(contextIds).ToArray();
            return actionBases.Where(@base => ids.Contains(@base.Id));
        }

        private IEnumerable<string> GetActionContextIds(string actionContext){
            if (!string.IsNullOrEmpty(actionContext)){
                var modelActionContexts = ((IModelApplicationModelArtifactState) Application.Model).ModelArtifactState.ConditionalActionState.ActionContexts.First(contexts => contexts.Id==actionContext);
                return modelActionContexts.Select(link => link.ActionId);
            }
            return Enumerable.Empty<string>();
        }

        void ActivateDeActivateAction(LogicRuleInfo info, ActionBase actionBase) {
            actionBase.Active[ActiveObjectTypeHasActionRules] = info.InvertCustomization || ((IActionStateRule)info.Rule).ActionState != ActionState.Hidden;
        }

        void EnableDisableAction(LogicRuleInfo info, ActionBase actionBase) {
            actionBase.Enabled[ActiveObjectTypeHasActionRules] = info.InvertCustomization || ((IActionStateRule)info.Rule).ActionState != ActionState.Disabled;
        }

        void ExecuteAction(ActionBase actionBase) {
            if (!(actionBase is SimpleAction))
                throw new NotSupportedException(actionBase.GetType().ToString());
            var simpleAction = ((SimpleAction)actionBase);
            if (simpleAction.Active && simpleAction.Enabled) {
                simpleAction.DoExecute();
            }
        }

        void ExecuteAndDisableAction(ActionBase actionBase) {
            var simpleAction = ((SimpleAction)actionBase);
            simpleAction.Active[ActiveObjectTypeHasActionRules] = true;
            if (simpleAction.Active && simpleAction.Enabled)
                simpleAction.DoExecute();
            simpleAction.Active[ActiveObjectTypeHasActionRules] = false;
        }

        IEnumerable<ActionBase> GetModuleActions(IActionStateRule rule, IEnumerable<ActionBase> actionBases) {
            var assemblies =Application.Modules.Where(@base => new Regex(rule.Module).IsMatch(@base.GetType().FullName + "")).Select(
                    @base => @base.GetType().Assembly.FullName).ToArray();
            return actionBases.Where(@base => assemblies.Contains(@base.Controller.GetType().Assembly.FullName));
        }

        void OnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs logicRuleExecuteEventArgs) {
            var logicRuleInfo = logicRuleExecuteEventArgs.LogicRuleInfo;
            if (logicRuleInfo.Active){
                var rule = logicRuleInfo.Rule as IActionStateRule;
                if (rule != null){
                    foreach (ActionBase actionBase in GetActions(rule)){
                        switch (rule.ActionState){
                            case ActionState.ForceActive:{
                                actionBase.Active.Clear();
                                break;
                            }
                            case ActionState.Hidden:
                                ActivateDeActivateAction(logicRuleInfo, actionBase);
                                break;
                            case ActionState.Disabled:
                                EnableDisableAction(logicRuleInfo, actionBase);
                                break;
                            case ActionState.Executed:{
                                if (logicRuleInfo.Active){
                                    ExecuteAction(actionBase);
                                }
                            }
                                break;
                            case ActionState.ExecutedAndDisable:
                                ExecuteAndDisableAction(actionBase);
                                break;
                        }
                    }
                }
            }

        }
    }
}


