using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic {
    public interface ILogicModuleBase {
        IModelLogic GetModelLogic(IModelNode applicationModel);
    }

    public abstract class LogicModuleBase<TLogicRule, TLogicRule2, TModelLogicRule, TModelApplication, TModelLogic> : XpandModuleBase, IRuleHolder, IRuleCollector, ILogicModuleBase where TModelLogic : IModelLogic
        where TModelLogicRule : IModelLogicRule
        where TLogicRule : ILogicRule
        where TLogicRule2 : ILogicRule where TModelApplication : IModelNode {
        public event EventHandler RulesCollected;

        protected void OnRulesCollected(EventArgs e) {
            EventHandler handler = RulesCollected;
            if (handler != null) handler(this, e);
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelApplication, TModelApplication>();
        }

        protected LogicModuleBase() {
            RequiredModuleTypes.Add(typeof(LogicModule));
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete += (sender, args) => CollectRules((XafApplication)sender);
            application.LoggedOn += (o, eventArgs) => CollectRules((XafApplication)o);
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new LogicDefaultContextNodeUpdater<TModelLogic,TModelApplication>(ExecutionContexts, GetModelLogic));
            updaters.Add(new LogicDefaultGroupContextNodeUpdater<TModelLogic, TModelApplication>(GetModelLogic));
            updaters.Add(LogicRulesNodeUpdater);
        }

        public abstract List<ExecutionContext>  ExecutionContexts { get; }

        
        public abstract LogicRulesNodeUpdater<TLogicRule, TModelLogicRule, TModelApplication> LogicRulesNodeUpdater { get; }
        
        public virtual void CollectRules(XafApplication xafApplication) {
            lock (LogicRuleManager<TLogicRule>.Instance) {
                bool reloadPermissions = ReloadPermissions();
                var modelLogic = GetModelLogic(xafApplication.Model);
                foreach (ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes) {
                    LogicRuleManager<TLogicRule>.Instance[typeInfo] = null;
                    var modelLogicRules = CollectRulesFromModel(modelLogic, typeInfo).ToList();
                    var permissionsLogicRules = CollectRulesFromPermissions(modelLogic, typeInfo, reloadPermissions).ToList();
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
            return reloadPermissions ? (IEnumerable<TLogicRule>)GetPermissions().OfType<TLogicRule>().Where(permission =>
                           permission.TypeInfo != null && permission.TypeInfo.Type == typeInfo.Type).OrderBy(rule => rule.Index)
                       : new List<TLogicRule>();
        }

        IEnumerable GetPermissions() {
            object user = SecuritySystem.CurrentUser as IUser;
            if (user != null) {
                return ((IUser)SecuritySystem.CurrentUser).Permissions.ToList();
            }
            user = SecuritySystem.CurrentUser as ISecurityUserWithRoles;
            if (user != null)
                return ((ISecurityUserWithRoles)SecuritySystem.CurrentUser).GetPermissions();
            throw new NotImplementedException(SecuritySystem.CurrentUser.GetType().FullName);
        }

        IEnumerable<TLogicRule> CollectRulesFromModel(IModelLogic modelLogic, ITypeInfo info) {
            return (modelLogic.Rules.Where(ruleDefinition => info == ruleDefinition.ModelClass.TypeInfo).Select(GetRuleObject)).OfType<TLogicRule>();
        }

        protected virtual TLogicRule2 GetRuleObject(IModelLogicRule modelLogicRule) {
            var logicRule2 = ((TLogicRule2)ReflectionHelper.CreateObject(typeof(TLogicRule2), (TLogicRule)modelLogicRule));
            logicRule2.TypeInfo = modelLogicRule.ModelClass.TypeInfo;
            return logicRule2;
        }

        public abstract TModelLogic GetModelLogic(TModelApplication applicationModel);

        public IModelLogic GetModelLogic(IModelNode applicationModel) {
            return GetModelLogic((TModelApplication) applicationModel);
        }

        public bool HasRules(View view) {
            return LogicRuleManager<TLogicRule>.HasRules(view);
        }
    }
}