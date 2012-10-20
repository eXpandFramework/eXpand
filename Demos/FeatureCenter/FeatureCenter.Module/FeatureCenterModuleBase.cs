using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.AdditionalViewControlsProvider;

namespace FeatureCenter.Module {
    public abstract class FeatureCenterModuleBase : ModuleBase {
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            var additionalViewControlsModule = (AdditionalViewControlsModule)moduleManager.Modules.FindModule(typeof(AdditionalViewControlsModule));
            additionalViewControlsModule.RulesCollected += AdditionalViewControlsModuleOnRulesCollected;
        }

        protected abstract void AdditionalViewControlsModuleOnRulesCollected(object sender, EventArgs e);
    }
}