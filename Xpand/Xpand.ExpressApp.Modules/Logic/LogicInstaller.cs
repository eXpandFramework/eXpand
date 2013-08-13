using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Logic {
    public class LogicInstallerManager {
        readonly List<ILogicInstaller> _logicInstallers=new List<ILogicInstaller>(); 
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

        public List<ILogicInstaller> LogicInstallers {
            get { return _logicInstallers; }
        }

        public void RegisterInstaller(ILogicInstaller logicInstaller) {
            _logicInstallers.Add(logicInstaller);
        }

        public void RegisterInstallers(IEnumerable<ILogicInstaller> logicInstallers) {
            _logicInstallers.AddRange(logicInstallers);        
        }
    }

    public interface ILogicInstaller {
        List<ExecutionContext> ExecutionContexts { get; }
        IModelLogic GetModelLogic(IModelApplication applicationModel);
    }

    public abstract class LogicInstaller<TLogicRule,  TModelLogicRule> : ILogicInstaller where TModelLogicRule : IModelLogicRule
        where TLogicRule : ILogicRule {
        readonly XpandModuleBase _module;
        XafApplication _application;

        protected LogicInstaller(XpandModuleBase xpandModuleBase) {
            _module = xpandModuleBase;
            _module.CustomAddGeneratorUpdaters += ModuleOnCustomAddGeneratorUpdaters;
            _module.ApplicationModulesManagerSetup+=ModuleOnApplicationModulesManagerSetup;
            xpandModuleBase.RequiredModuleTypes.Add(typeof(LogicModule));
        }

        void ModuleOnApplicationModulesManagerSetup(object sender, EventArgs eventArgs) {
            _module.ApplicationModulesManagerSetup-=OnApplicationOnSetupComplete;
            if (InterfaceBuilder.RuntimeMode) {
                _application = _module.Application;
                _module.Application.Modules.FindModule<LogicModule>().LogicRuleCollector.CollectModelLogics+=LogicRuleCollectorOnCollectModelLogics;
                _application.SetupComplete += OnApplicationOnSetupComplete;
            }
        }

        void LogicRuleCollectorOnCollectModelLogics(object sender, CollectModelLogicsArgs collectModelLogicsArgs) {
            ((LogicRuleCollector) sender).CollectModelLogics-=LogicRuleCollectorOnCollectModelLogics;
            collectModelLogicsArgs.ModelLogics.Add(GetModelLogic(_application.Model.Application));
        }

        void OnApplicationOnSetupComplete(object sender, EventArgs args) {
            _application.SetupComplete-=OnApplicationOnSetupComplete;
            IModelLogic modelLogic = GetModelLogic(_application.Model);
            LogicRuleEvaluator.ModelLogics.Add(modelLogic);
        }

        void ModuleOnCustomAddGeneratorUpdaters(object sender, GeneratorUpdaterEventArgs generatorUpdaterEventArgs) {
            _module.CustomAddGeneratorUpdaters-=ModuleOnCustomAddGeneratorUpdaters;
            var updaters = generatorUpdaterEventArgs.Updaters;
            updaters.Add(new LogicDefaultContextNodeUpdater(ExecutionContexts, GetModelLogic));
            updaters.Add(new LogicDefaultGroupContextNodeUpdater(GetModelLogic));
            updaters.Add(LogicRulesNodeUpdater);
        }

        public abstract List<ExecutionContext>  ExecutionContexts { get; }

        public abstract LogicRulesNodeUpdater<TLogicRule, TModelLogicRule> LogicRulesNodeUpdater { get; }

        public IModelLogic GetModelLogic() {
            return GetModelLogic(_application.Model);
        }

        public abstract IModelLogic GetModelLogic(IModelApplication applicationModel);
    }
}