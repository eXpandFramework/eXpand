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
using Xpand.ExpressApp.Security.Core;

namespace Xpand.ExpressApp.Logic {
    public abstract class LogicModuleBase<TLogicRule, TLogicRule2> : XpandModuleBase, IRuleHolder, IRuleCollector
        where TLogicRule : ILogicRule
        where TLogicRule2 : ILogicRule {
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
            application.SetupComplete += (sender, args) => CollectRules((XafApplication)sender);
            application.LoggedOn += (o, eventArgs) => CollectRules((XafApplication)o);
        }

        public virtual void CollectRules(XafApplication xafApplication) {
            lock (LogicRuleManager<TLogicRule>.Instance) {
                bool reloadPermissions = ReloadPermissions();
                IModelLogic modelLogic = GetModelLogic(xafApplication.Model);
                foreach (ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes) {
                    LogicRuleManager<TLogicRule>.Instance[typeInfo] = null;
                    List<TLogicRule> modelLogicRules = CollectRulesFromModel(modelLogic, typeInfo).ToList();
                    List<TLogicRule> permissionsLogicRules = CollectRulesFromPermissions(modelLogic, typeInfo, reloadPermissions).ToList();
                    modelLogicRules.AddRange(permissionsLogicRules);
                    LogicRuleManager<TLogicRule>.Instance[typeInfo] = modelLogicRules;
                }
            }
            OnRulesCollected(EventArgs.Empty);
        }

        bool ReloadPermissions() {
            if (SecuritySystem.Instance is ISecurityComplex)
                if (SecuritySystem.CurrentUser != null) {
                    SecuritySystem.ReloadPermissions();
                    return true;
                }
            return false;
        }

        protected virtual IEnumerable<TLogicRule> CollectRulesFromPermissions(IModelLogic modelLogic, ITypeInfo typeInfo, bool reloadPermissions) {
            if (reloadPermissions) {
                if (!((ISecurityComplex)SecuritySystem.Instance).IsNewSecuritySystem()) {
                    IList<IPermission> permissions = ((IUser)SecuritySystem.CurrentUser).Permissions;
                    return permissions.OfType<TLogicRule>().Where(permission => permission.TypeInfo != null && permission.TypeInfo.Type == typeInfo.Type).OrderBy(rule => rule.Index);
                }
            }
            return new List<TLogicRule>();
        }

        IEnumerable<TLogicRule> CollectRulesFromModel(IModelLogic modelLogic, ITypeInfo info) {
            return (modelLogic.Rules.Where(
                ruleDefinition => info == ruleDefinition.ModelClass.TypeInfo).Select(GetRuleObject)).OfType<TLogicRule>();
        }


        protected virtual TLogicRule2 GetRuleObject(IModelLogicRule modelLogicRule) {
            var logicRule2 = ((TLogicRule2)ReflectionHelper.CreateObject(typeof(TLogicRule2), (TLogicRule)modelLogicRule));
            logicRule2.TypeInfo = modelLogicRule.ModelClass.TypeInfo;
            return logicRule2;
        }

        protected abstract IModelLogic GetModelLogic(IModelApplication applicationModel);

        public bool HasRules(View view) {
            return LogicRuleManager<TLogicRule>.HasRules(view);
        }
    }
}