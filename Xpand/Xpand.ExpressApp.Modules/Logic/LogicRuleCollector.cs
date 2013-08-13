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

        protected virtual void CollectRules(IEnumerable<ILogicRule> logicRules) {
            var ruleObjects = logicRules.Select(CreateRuleObject);
            var groupings = ruleObjects.GroupBy(rule => rule.TypeInfo).Select(rules => new{rules.Key, Rules = rules.ToList()});
            foreach (var grouping in groupings) {
                var typeInfo = grouping.Key;
                var rules = LogicRuleManager.Instance[typeInfo];
                rules.AddRange(grouping.Rules);
                foreach (var info in typeInfo.Descendants) {
                    LogicRuleManager.Instance[info].AddRange(grouping.Rules);
                }
            }
        }

        protected virtual ILogicRule CreateRuleObject(ILogicRule modelLogicRule) {
            var logicRuleObjectType = LogicRuleObjectType(modelLogicRule);
            var logicRuleObject = ((LogicRule)ReflectionHelper.CreateObject(logicRuleObjectType, modelLogicRule));
            logicRuleObject.TypeInfo = modelLogicRule.TypeInfo;
            return logicRuleObject;
        }

        Type LogicRuleObjectType(ILogicRule modelLogicRule) {
            var typesInfo = _module.Application.TypesInfo;
            var type = ConcreteType(modelLogicRule,typesInfo);
            return typesInfo.FindTypeInfo<LogicRule>().Descendants.Single(info => type.IsAssignableFrom(info.Type)).Type;
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
                foreach (var modelLogic in _modelLogics) {
                    CollectRules(modelLogic.Rules);
                }
                CollectRules(GetPermissions());
            }
            OnRulesCollected(EventArgs.Empty);
        }

        void AddModelLogics() {
            var collectModelLogicsArgs = new CollectModelLogicsArgs();
            OnCollectModelLogics(collectModelLogicsArgs);
            if (collectModelLogicsArgs.ModelLogic != null && !_modelLogics.Contains(collectModelLogicsArgs.ModelLogic))
                _modelLogics.Add(collectModelLogicsArgs.ModelLogic);
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
        public IModelLogic ModelLogic { get; set; }
    }
}