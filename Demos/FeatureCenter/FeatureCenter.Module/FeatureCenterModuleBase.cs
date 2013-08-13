using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Logic;

namespace FeatureCenter.Module {
    public abstract class FeatureCenterModuleBase : ModuleBase {
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            var logicModule = moduleManager.Modules.FindModule<LogicModule>();
            logicModule.LogicRuleCollector.RulesCollected += AdditionalViewControlsModuleOnRulesCollected;
        }

        protected abstract void AdditionalViewControlsModuleOnRulesCollected(object sender, EventArgs e);
    }
}