using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleCollector {
        internal static bool PermissionsReloaded;
        ModuleBase _module;
        public event EventHandler RulesCollected;
        public event EventHandler<CollectModelLogicsArgs> CollectModelLogics;

        protected virtual void OnCollectModelLogics(CollectModelLogicsArgs e) {
            EventHandler<CollectModelLogicsArgs> handler = CollectModelLogics;
            if (handler != null) handler(this, e);
        }

        readonly HashSet<IModelLogic> _modelLogics=new HashSet<IModelLogic>(); 

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            _module.Application.SetupComplete -= ApplicationOnSetupComplete;
            CollectRules((XafApplication)sender);
        }

        protected virtual IEnumerable<ILogicRule> CollectRulesFromPermissions(IModelLogic modelLogic, ITypeInfo typeInfo) {
            return  GetPermissions().Where(permission =>permission.TypeInfo != null && permission.TypeInfo.Type == typeInfo.Type)
                .OrderBy(rule => rule.Index);
        }

        IEnumerable<ILogicRule> GetPermissions() {
            object user = SecuritySystem.CurrentUser as IUser;
            if (user != null) {
                return ((IUser)SecuritySystem.CurrentUser).Permissions.OfType<ILogicRule>();
            }
            user = SecuritySystem.CurrentUser as ISecurityUserWithRoles;
            return user != null ? ((ISecurityUserWithRoles)SecuritySystem.CurrentUser).GetPermissions().OfType<ILogicRule>() 
                : Enumerable.Empty<ILogicRule>();
        }

        void ReloadPermissions() {
            if (SecuritySystem.Instance is ISecurityComplex)
                if (SecuritySystem.CurrentUser != null && !PermissionsReloaded) {
                    SecuritySystem.ReloadPermissions();
                    PermissionsReloaded = true;
                }
        }

        protected virtual LogicRule CreateRuleObject(ILogicRule logicRule, IModelLogic modelLogic) {
            var logicRuleObjectType = LogicRuleObjectType(logicRule);
            var logicRuleObject = ((LogicRule)ReflectionHelper.CreateObject(logicRuleObjectType, logicRule));
            logicRuleObject.TypeInfo = logicRule.TypeInfo;
            logicRuleObject.ExecutionContext = GetExecutionContext(logicRule, modelLogic);
            return logicRuleObject;
        }

        ExecutionContext GetExecutionContext(ILogicRule logicRule, IModelLogic modelLogic) {
            return modelLogic.ExecutionContextsGroup.First(contexts => contexts.Id==logicRule.ExecutionContextGroup).ExecutionContext;
        }

        Type LogicRuleObjectType(ILogicRule modelLogicRule) {
            var typesInfo = _module.Application.TypesInfo;
            var type = ConcreteType(modelLogicRule,typesInfo);
            return typesInfo.FindTypeInfo<LogicRule>().Descendants.Single(info => !info.Type.IsAbstract&&type.IsAssignableFrom(info.Type)).Type;
        }

        Type ConcreteType(ILogicRule logicRule, ITypesInfo typesInfo) {
            var logicRuleTypeInfo = typesInfo.FindTypeInfo(logicRule.GetType());
            return logicRuleTypeInfo.ImplementedInterfaces.Select(info=> info.FindAttribute<ModelInterfaceImplementorAttribute>())
                               .Where(attribute=> attribute != null).Select(attribute => attribute.ImplementedInterface).Single();
        }

        void OnRulesCollected(EventArgs e) {
            EventHandler handler = RulesCollected;
            if (handler != null) handler(this, e);
        }

        public virtual void CollectRules(XafApplication xafApplication) {
            AddModelLogics();
            lock (LogicRuleManager.Instance) {
                ReloadPermissions();
                LogicRuleManager.Instance.Rules.Clear();
                var logicTypes = new Dictionary<Type, IModelLogic>();
                foreach (var modelLogic in _modelLogics) {
                    CollectRules(modelLogic.Rules,modelLogic);
                    var ruleType = _module.Application.TypesInfo.FindTypeInfo(modelLogic.GetType()).FindAttributes<ModelLogicValidRuleAttribute>().First().RuleType;
                    logicTypes.Add(ruleType, modelLogic);
                }
                var groupings = GetPermissions().Select(rule => new{rule,Type=rule.GetType()}).
                    GroupBy(arg => arg.Type).Select(grouping 
                        => new { ModelLogic = GetModelLogic(grouping.Key,logicTypes), Rules = grouping.Select(arg => arg.rule) });
                foreach (var grouping in groupings) {
                    CollectRules(grouping.Rules, grouping.ModelLogic);
                }
            }
            OnRulesCollected(EventArgs.Empty);
        }

        protected virtual void CollectRules(IEnumerable<ILogicRule> logicRules, IModelLogic modelLogic) {
            var ruleObjects = logicRules.Select(rule => CreateRuleObject(rule, modelLogic));
            var groupings = ruleObjects.GroupBy(rule => rule.TypeInfo).Select(grouping => new { grouping.Key, Rules = grouping });
            foreach (var grouping in groupings) {
                var typeInfo = grouping.Key;
                var rules = LogicRuleManager.Instance[typeInfo];
                IGrouping<ITypeInfo, LogicRule> collection = grouping.Rules;
                rules.AddRange(collection);
                foreach (var info in typeInfo.Descendants) {
                    LogicRuleManager.Instance[info].AddRange(collection);
                }
            }
        }

        IModelLogic GetModelLogic(Type type, Dictionary<Type, IModelLogic> modelLogics) {
            return modelLogics.First(pair => pair.Key.IsInstanceOfType(type)).Value;
        }

        void AddModelLogics() {
            var collectModelLogicsArgs = new CollectModelLogicsArgs();
            OnCollectModelLogics(collectModelLogicsArgs);
            var modelLogics = collectModelLogicsArgs.ModelLogics.Where(modelLogic => !_modelLogics.Contains(modelLogic));
            foreach (var modelLogic in modelLogics) {
                _modelLogics.Add(modelLogic);
            }
        }

        public void Attach(ModuleBase moduleBase) {
            _module = moduleBase;
            if (moduleBase.Application != null) {
                moduleBase.Application.LoggedOn += (o, eventArgs) => CollectRules((XafApplication)o);
                moduleBase.Application.SetupComplete += ApplicationOnSetupComplete;
            }
        }
    }

    public class CollectModelLogicsArgs : EventArgs {
        readonly List<IModelLogic> _modelLogics=new List<IModelLogic>();
        public List<IModelLogic> ModelLogics {
            get { return _modelLogics; }
        }
    }
}