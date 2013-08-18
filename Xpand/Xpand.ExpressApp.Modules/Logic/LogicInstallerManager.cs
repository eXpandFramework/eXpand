using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic;
using System.Linq;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.ModelDifference;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Logic {
    public class LogicInstallerManager {
        readonly List<ILogicInstaller> _logicInstallers = new List<ILogicInstaller>();
        readonly Dictionary<Type, ILogicInstaller> _logicInstallerTypes = new Dictionary<Type, ILogicInstaller>();
        static IValueManager<LogicInstallerManager> _instanceManager;

        LogicInstallerManager() {
        }

        public static LogicInstallerManager Instance {
            get {
                if (_instanceManager == null) {
                    _instanceManager = ValueManager.GetValueManager<LogicInstallerManager>("LogicInstallerManager");
                }
                return _instanceManager.Value ?? (_instanceManager.Value = new LogicInstallerManager());
            }
        }

        public ReadOnlyCollection<ILogicInstaller> LogicInstallers {
            get { return _logicInstallers.AsReadOnly(); }
        }

        public ILogicInstaller this[IModelLogicRule logicRule] {
            get {
                var typeInfo = ((ITypesInfoProvider) logicRule.Application).TypesInfo.FindTypeInfo(logicRule.GetType());
                var memberType = typeInfo.FindMember<IModelConditionalLogicRule<ILogicRule>>(rule => rule.Attribute).MemberType;
                return this[memberType,logicRule.Application];
            }
        }

        ILogicInstaller this[Type ruleType,IModelApplication application ] {
            get {
                if (!_logicInstallerTypes.ContainsKey(ruleType)) {
                    _logicInstallerTypes[ruleType] = _logicInstallers.First(installer =>
                        ruleType == installer.GetModelLogic(application).RuleType);
                }
                return _logicInstallerTypes[ruleType];

            }
        }

        public ILogicInstaller this[Type ruleType] {
            get {
                if (!_logicInstallerTypes.ContainsKey(ruleType)) {
                    _logicInstallerTypes[ruleType] = _logicInstallers.First(installer =>
                        ruleType == installer.GetModelLogic().RuleType);
                }
                return _logicInstallerTypes[ruleType];
            }
        }
        
        public static void RegisterInstaller(ILogicInstaller logicInstaller) {
            Instance._logicInstallers.Add(logicInstaller);
        }

        public static void RegisterInstallers(IEnumerable<ILogicInstaller> logicInstallers) {
            Instance._logicInstallers.AddRange(logicInstallers);
        }
    }
}