using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleManager {
        static IValueManager<LogicRuleManager> _instanceManager;
        readonly Dictionary<Tuple<ITypeInfo,ExecutionContext>,List<ILogicRuleObject>> _rules;

        LogicRuleManager() {
            _rules=new Dictionary<Tuple<ITypeInfo, ExecutionContext>, List<ILogicRuleObject>>();
        }

        public static LogicRuleManager Instance {
            get {
                if (_instanceManager == null) {
                    _instanceManager = ValueManager.GetValueManager<LogicRuleManager>("LogicRuleManager");
                }
                return _instanceManager.Value ?? (_instanceManager.Value = new LogicRuleManager());
            }
        }

        public ReadOnlyCollection<ILogicRuleObject> this[Tuple<ITypeInfo,ExecutionContext> tuple] {
            get {
                lock (_rules) {
                    if (!_rules.ContainsKey(tuple)) {
                        _rules.Add(tuple, new List<ILogicRuleObject>());
                    }
                    return _rules[tuple].AsReadOnly();
                }
            }
        }
        public ReadOnlyCollection<ILogicRuleObject> this[ITypeInfo typeInfo] {
            get {
                var logicRuleObjects = new List<ILogicRuleObject>();
                foreach (var result in LogicInstallerManager.Instance.LogicInstallers.SelectMany(installer => installer.ExecutionContexts)) {
                    logicRuleObjects.AddRange(Instance[new Tuple<ITypeInfo, ExecutionContext>(typeInfo, result)]);
                }
                return logicRuleObjects.AsReadOnly();
            }
        }

        public static bool HasRules(ITypeInfo typeInfo) {
            var executionContexts = LogicInstallerManager.Instance.LogicInstallers.SelectMany(installer => installer.ExecutionContexts);
            return executionContexts.Any(context => Instance[new Tuple<ITypeInfo, ExecutionContext>(typeInfo, context)].Any());
        }

        public static bool HasRules<TLogicInstaller>(ITypeInfo typeInfo) where TLogicInstaller : ILogicInstaller {
            return LogicInstallerManager.Instance.LogicInstallers.OfType<TLogicInstaller>().First().ExecutionContexts.Any(context 
                => Instance[new Tuple<ITypeInfo, ExecutionContext>(typeInfo, context)].Any());
        }

        public static bool IsDisplayedMember(IMemberInfo memberInfo) {
            if (memberInfo.Owner.KeyMember == memberInfo) {
                return true;
            }
            return memberInfo.IsPublic && memberInfo.IsVisible &&
                   (memberInfo.MemberTypeInfo.IsDomainComponent || memberInfo.Owner.IsDomainComponent);
        }

        public static IEnumerable<ILogicRule> FindAttributes(ITypeInfo typeInfo) {
            return typeInfo != null ? GetLogicRuleAttributes(typeInfo) : null;
        }

        static IEnumerable<ILogicRule> GetLogicRuleAttributes(ITypeInfo typeInfo) {
            return typeInfo.FindAttributes<Attribute>(false).OfType<ILogicRule>();
        }

        public void ClearAllRules() {
            _rules.Clear();
            _rules.Clear();
        }

        public void AddRules(ITypeInfo typeInfo, IEnumerable<ILogicRuleObject> rules) {
            foreach (var logicRuleObject in rules) {
                AddRuleCore(typeInfo,ExecutionContext.None,logicRuleObject);
                var executionContexts = logicRuleObject.ExecutionContext.GetIndividualValues<ExecutionContext>();
                foreach (var executionContext in executionContexts) {
                    AddRuleCore(typeInfo, executionContext, logicRuleObject);
                }                
            }
        }

        void AddRuleCore(ITypeInfo typeInfo, ExecutionContext executionContext, ILogicRuleObject logicRuleObject) {
            var tuple = new Tuple<ITypeInfo, ExecutionContext>(typeInfo, executionContext);
            if (!_rules.ContainsKey(tuple))
                _rules.Add(tuple, new List<ILogicRuleObject>());
            _rules[tuple].Add(logicRuleObject);
        }
    }
}