using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Logic {
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
            _module.ApplicationModulesManagerSetup-=ModuleOnApplicationModulesManagerSetup;
            if (InterfaceBuilder.RuntimeMode) {
                _application = _module.Application;
                _module.Application.Modules.FindModule<LogicModule>().LogicRuleCollector.CollectModelLogics+=LogicRuleCollectorOnCollectModelLogics;
            }
        }

        void LogicRuleCollectorOnCollectModelLogics(object sender, CollectModelLogicsArgs collectModelLogicsArgs) {
            ((LogicRuleCollector) sender).CollectModelLogics-=LogicRuleCollectorOnCollectModelLogics;
            collectModelLogicsArgs.ModelLogics.Add(GetModelLogic(_application.Model.Application));
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

        public IModelLogicWrapper GetModelLogic() {
            return GetModelLogic(_application.Model);
        }

        protected abstract IModelLogicWrapper GetModelLogicCore(IModelApplication applicationModel);

        public IModelLogicWrapper GetModelLogic(IModelApplication applicationModel) {
            var modelLogicWrapper = GetModelLogicCore(applicationModel);
            modelLogicWrapper.RuleType = typeof(TLogicRule);        
            return modelLogicWrapper;
        }
    }

}