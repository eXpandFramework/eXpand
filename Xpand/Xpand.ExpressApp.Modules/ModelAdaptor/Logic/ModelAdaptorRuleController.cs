using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.ModelAdapter.Logic;

namespace Xpand.ExpressApp.ModelAdaptor.Logic {
    public class ModelAdaptorRuleController : ViewController, IModelAdaptorRuleController {
        readonly Dictionary<Type,List<IModelNodeEnabled>> _ruleTypeActiveModels = new Dictionary<Type, List<IModelNodeEnabled>>();
        LogicRuleViewController _logicRuleViewController;

        public Dictionary<Type, List<IModelNodeEnabled>> RuleTypeActiveModels {
            get { return _ruleTypeActiveModels; }
        }
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.Disposing += FrameOnDisposing;
            _logicRuleViewController = Frame.GetController<LogicRuleViewController>();
            _logicRuleViewController.LogicRuleExecute += OnLogicRuleExecute;
        }

        void OnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs e) {
            if (e.LogicRuleInfo.Active) {
                var modelAdaptorRule = e.LogicRuleInfo.Rule as IModelAdaptorRule;
                if (modelAdaptorRule!=null) {
                    if (!_ruleTypeActiveModels.ContainsKey(modelAdaptorRule.RuleType))
                        _ruleTypeActiveModels.Add(modelAdaptorRule.RuleType, new List<IModelNodeEnabled>());
                    var modelNodeEnableds = _ruleTypeActiveModels[modelAdaptorRule.RuleType];
                    var modelAdaptorLogicInstaller = LogicInstallerManager.Instance.LogicInstallers.OfType<ModelAdaptorLogicInstaller>().Single();
                    var modelLogicRules =modelAdaptorLogicInstaller.GetModelLogic().Rules;
                    var modelLogicRule = modelLogicRules[modelAdaptorRule.Id];
                    modelNodeEnableds.Add((IModelNodeEnabled) modelLogicRule);
                }
            }            
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            _logicRuleViewController.LogicRuleExecute -= OnLogicRuleExecute;
        }
        
        public void ExecuteLogic<TModelAdaptorRule, TModelModelAdaptorRule>(Action<TModelModelAdaptorRule> action)
            where TModelAdaptorRule : IModelAdaptorRule
            where TModelModelAdaptorRule : IModelModelAdaptorRule {
            ((IModelAdaptorRuleController) this).ExecuteLogic(typeof(TModelAdaptorRule),typeof(TModelModelAdaptorRule),rule => action.Invoke((TModelModelAdaptorRule) rule));
        }
        
        void IModelAdaptorRuleController.ExecuteLogic(Type modelAdaptorRuleType, Type modelModelAdaptorRuleType, Action<IModelAdaptorRule> action) {
            var type = modelAdaptorRuleType;
            if (RuleTypeActiveModels.ContainsKey(type)) {
                var activeModels = RuleTypeActiveModels[type];
                foreach (var modelNodeEnabled in activeModels.ToList().Where(enabled => "I"+enabled.GetType().Name==modelModelAdaptorRuleType.Name)) {
                    action.Invoke((IModelAdaptorRule)modelNodeEnabled);
                    activeModels.Remove(modelNodeEnabled);
                }
                RuleTypeActiveModels.Remove(type);
            }
        
        }
    }
//    public class ModelAdaptorRuleController : ConditionalLogicRuleViewController<IModelAdaptorRule, ModelAdaptorModule>, IModelAdaptorRuleController {
//        readonly Dictionary<Type,List<IModelNodeEnabled>> _ruleTypeActiveModels = new Dictionary<Type, List<IModelNodeEnabled>>();
//
//        public Dictionary<Type, List<IModelNodeEnabled>> RuleTypeActiveModels {
//            get { return _ruleTypeActiveModels; }
//        }
//
//        public override void ExecuteRule(LogicRuleInfo<IModelAdaptorRule> info, ExecutionContext executionContext) {
//            if (info.Active) {
//                if (!_ruleTypeActiveModels.ContainsKey(info.Rule.RuleType))
//                    _ruleTypeActiveModels.Add(info.Rule.RuleType, new List<IModelNodeEnabled>());
//                List<IModelNodeEnabled> modelNodeEnableds = _ruleTypeActiveModels[info.Rule.RuleType];
//                var modelAdaptorModule = Application.Modules.FindModule<ModelAdaptorModule>();
//                IModelLogicRules modelLogicRules = modelAdaptorModule.GetModelLogic(Application.Model).Rules;
//                IModelLogicRule modelLogicRule = modelLogicRules[info.Rule.Id];
//                modelNodeEnableds.Add((IModelNodeEnabled) modelLogicRule);
//            }            
//        }
//
//        public void ExecuteLogic<TModelAdaptorRule, TModelModelAdaptorRule>(Action<TModelModelAdaptorRule> action)
//            where TModelAdaptorRule : IModelAdaptorRule
//            where TModelModelAdaptorRule : IModelModelAdaptorRule {
//            ((IModelAdaptorRuleController) this).ExecuteLogic(typeof(TModelAdaptorRule),typeof(TModelModelAdaptorRule),rule => action.Invoke((TModelModelAdaptorRule) rule));
//        }
//
//        void IModelAdaptorRuleController.ExecuteLogic(Type modelAdaptorRuleType, Type modelModelAdaptorRuleType, Action<IModelAdaptorRule> action) {
//            var type = modelAdaptorRuleType;
//            if (RuleTypeActiveModels.ContainsKey(type)) {
//                var activeModels = RuleTypeActiveModels[type];
//                foreach (var modelNodeEnabled in activeModels.ToList().Where(enabled => "I"+enabled.GetType().Name==modelModelAdaptorRuleType.Name)) {
//                    action.Invoke((IModelAdaptorRule)modelNodeEnabled);
//                    activeModels.Remove(modelNodeEnabled);
//                }
//                RuleTypeActiveModels.Remove(type);
//            }
//
//        }
//    }

}