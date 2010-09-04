using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp;

namespace Xpand.ExpressApp.Logic {
    public abstract class LogicModuleBase<TLogicRule, TLogicRule2> : XpandModuleBase,IRuleHolder,IRuleCollector
        where TLogicRule : ILogicRule
        where TLogicRule2 : ILogicRule{
        public event EventHandler RulesCollected;

        protected void OnRulesCollected(EventArgs e) {
            EventHandler handler = RulesCollected;
            if (handler != null) handler(this, e);
        }

        protected LogicModuleBase() {
            RequiredModuleTypes.Add(typeof(LogicModule));
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete += (sender, args) => CollectRules(application);
            application.LoggedOn += (o, eventArgs) => CollectRules(application);
        }

        public virtual void CollectRules(XafApplication xafApplication) {
            lock (LogicRuleManager<TLogicRule>.Instance){
                IModelLogic modelLogic = GetModelLogic(xafApplication.Model);
                foreach (ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes){
                    LogicRuleManager<TLogicRule>.Instance[typeInfo] = null;
                    List<TLogicRule> modelLogicRules = CollectRulesFromModel(modelLogic, typeInfo).ToList();
                    List<TLogicRule> permissionsLogicRules = CollectRulesFromPermissions(modelLogic, typeInfo).ToList();
                    modelLogicRules.AddRange(permissionsLogicRules);
                    LogicRuleManager<TLogicRule>.Instance[typeInfo] = modelLogicRules;
                }
            }
            OnRulesCollected(EventArgs.Empty);
        }
        
        protected virtual IEnumerable<TLogicRule> CollectRulesFromPermissions(IModelLogic modelLogic, ITypeInfo typeInfo) {
            if (SecuritySystem.Instance is ISecurityComplex)
                if (SecuritySystem.CurrentUser != null){
                    SecuritySystem.ReloadPermissions();
                    IList<IPermission> permissions = ((IUser)SecuritySystem.CurrentUser).Permissions;
                    var rulesFromPermissions = permissions.OfType<TLogicRule>().Where(permission 
                        =>permission.TypeInfo != null &&permission.TypeInfo.Type == typeInfo.Type).OfType<TLogicRule>();
                    return rulesFromPermissions.OrderBy(rule => rule.Index);
                }
            return new List<TLogicRule>();
        }

        IEnumerable<TLogicRule> CollectRulesFromModel(IModelLogic modelConditionalEditorStates, ITypeInfo info) {
            return (modelConditionalEditorStates.Rules.Where(
                ruleDefinition => info == ruleDefinition.ModelClass.TypeInfo).Select(rule => GetRuleObject(rule))).OfType<TLogicRule>();
        }


        protected virtual TLogicRule2 GetRuleObject(IModelLogicRule ruleDefinition) {
            var logicRule2 = ((TLogicRule2)ReflectionHelper.CreateObject(typeof(TLogicRule2), (TLogicRule)ruleDefinition));
            return logicRule2;
        }

        protected abstract IModelLogic GetModelLogic(IModelApplication applicationModel);

        public bool HasRules(View view) {
            return LogicRuleManager<TLogicRule>.HasRules(view);
        }
    }
}