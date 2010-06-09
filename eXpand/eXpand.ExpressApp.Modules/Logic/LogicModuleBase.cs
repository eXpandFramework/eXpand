using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.Logic {
    public abstract class LogicModuleBase<TLogicRule, TLogicRule2> : ModuleBase,IRuleHolder
        where TLogicRule : ILogicRule
        where TLogicRule2 : ILogicRule{

        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete += (sender, args) => CollectRules(application);
            application.LoggedOn += (o, eventArgs) => CollectRules(application);
        }

        public void CollectRules(XafApplication xafApplication) {
            CollectRulesFromModel(xafApplication.Model);
        }

        protected virtual IEnumerable<TLogicRule> GetRulesForType(IModelLogic modelConditionalEditorStates, ITypeInfo info) {
            return (modelConditionalEditorStates.Rules.Where(
                ruleDefinition => info == ruleDefinition.ModelClass.TypeInfo).Select(rule => GetRuleObject(rule))).OfType<TLogicRule>();
        }


        protected virtual TLogicRule2 GetRuleObject(IModelLogicRule ruleDefinition) {
            var logicRule2 = ((TLogicRule2) Activator.CreateInstance(typeof (TLogicRule2), (TLogicRule) ruleDefinition));
            logicRule2.TypeInfo = ruleDefinition.ModelClass.TypeInfo;
            return logicRule2;
        }

        protected virtual void CollectRulesFromModel(IModelApplication applicationModel) {
            lock (LogicRuleManager<TLogicRule>.Instance) {
                IModelLogic additionalViewControls =GetModelLogic(applicationModel);
                foreach (ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes){
                    LogicRuleManager<TLogicRule>.Instance[typeInfo] = null;
                    ITypeInfo info = typeInfo;
                    List<TLogicRule> rulesForType = GetRulesForType(additionalViewControls, info).ToList();
                    LogicRuleManager<TLogicRule>.Instance[typeInfo] = rulesForType;
                }
            }
        }

        protected abstract IModelLogic GetModelLogic(IModelApplication applicationModel);

        public bool HasRules(View view) {
            return LogicRuleManager<TLogicRule>.HasRules(view);
        }
    }
}