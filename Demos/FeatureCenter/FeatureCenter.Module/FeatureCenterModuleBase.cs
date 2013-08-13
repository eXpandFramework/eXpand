using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.AdditionalViewControlsProvider;
using Xpand.ExpressApp.Logic;

namespace FeatureCenter.Module {
    public abstract class FeatureCenterModuleBase : ModuleBase {
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            var additionalViewControlsModule = (LogicModule)moduleManager.Modules.FindModule(typeof(AdditionalViewControlsModule));
            additionalViewControlsModule.LogicRuleCollector.RulesCollected += AdditionalViewControlsModuleOnRulesCollected;
        }

        protected abstract void AdditionalViewControlsModuleOnRulesCollected(object sender, EventArgs e);
    }
}