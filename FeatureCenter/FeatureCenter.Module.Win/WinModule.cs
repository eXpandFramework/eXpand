using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;
using eXpand.ExpressApp.AdditionalViewControlsProvider;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.FilterDataStore.Core;
using eXpand.ExpressApp.FilterDataStore.Win.Providers;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.ModelDifference;
using CreateCustomModelDifferenceStoreEventArgs = eXpand.ExpressApp.ModelDifference.CreateCustomModelDifferenceStoreEventArgs;

namespace FeatureCenter.Module.Win
{
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class FeatureCenterWindowsFormsModule : ModuleBase
    {
        public FeatureCenterWindowsFormsModule()
        {
            InitializeComponent();
        }
        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            var modelDifferenceBaseModule = (ModelDifferenceBaseModule)moduleManager.Modules.Where(
                    mbase => typeof(ModelDifferenceBaseModule).IsAssignableFrom(mbase.GetType())).SingleOrDefault();
            if (modelDifferenceBaseModule != null)
                modelDifferenceBaseModule.CreateCustomModelDifferenceStore += ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore;
            var additionalViewControlsModule = (AdditionalViewControlsModule)moduleManager.Modules.FindModule(typeof(AdditionalViewControlsModule));
            additionalViewControlsModule.RulesCollected += AdditionalViewControlsModuleOnRulesCollected;
        }

        void AdditionalViewControlsModuleOnRulesCollected(object sender, EventArgs e) {
            foreach (var typeInfo in XafTypesInfo.Instance.PersistentTypes){
                var additionalViewControlsRules = LogicRuleManager<IAdditionalViewControlsRule>.Instance[typeInfo];
                foreach (var additionalViewControlsRule in additionalViewControlsRules){
                    if (additionalViewControlsRule.Id.StartsWith(Module.Captions.Header))
                        additionalViewControlsRule.ControlType = typeof(ApplicationCaption);
                    else if (additionalViewControlsRule.Id.StartsWith(Module.Captions.ViewMessage))
                        additionalViewControlsRule.ControlType = typeof(FooterMessagePanel);
                }    
            }
        }

        void ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e) {
            SkinFilterProvider skinFilterProvider = FilterProviderManager.Providers.OfType<SkinFilterProvider>().FirstOrDefault();
            if (skinFilterProvider != null)
                skinFilterProvider.FilterValue = ((IModelApplicationOptionsSkin)Application.Model.Options).Skin;
        }

    }
}
