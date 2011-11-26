using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using FeatureCenter.Module.Win.ApplicationDifferences.ExternalApplication;
using FeatureCenter.Module.Win.LowLevelFilterDataStore;
using FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyMasterDetail;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;
using Xpand.ExpressApp.ExceptionHandling.Win;
using Xpand.ExpressApp.FilterDataStore.Core;
using Xpand.ExpressApp.FilterDataStore.Win.Providers;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.ModelDifference;
using CreateCustomModelDifferenceStoreEventArgs = Xpand.ExpressApp.ModelDifference.CreateCustomModelDifferenceStoreEventArgs;

namespace FeatureCenter.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class FeatureCenterWindowsFormsModule : FeatureCenterModuleBase {
        public FeatureCenterWindowsFormsModule() {
            InitializeComponent();
            ParametersFactory.RegisterParameter(new ExternalApplicationKeyParameter());
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            var modelDifferenceBaseModule = (ModelDifferenceBaseModule)moduleManager.Modules.Where(
                    mbase => typeof(ModelDifferenceBaseModule).IsAssignableFrom(mbase.GetType())).FirstOrDefault();
            if (modelDifferenceBaseModule != null)
                modelDifferenceBaseModule.CreateCustomModelDifferenceStore += ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore;
            var exceptionHandlingWinModule =
                (ExceptionHandlingWinModule)moduleManager.Modules.FindModule(typeof(ExceptionHandlingWinModule));
            if (exceptionHandlingWinModule != null)
                exceptionHandlingWinModule.CustomHandleException += ExceptionHandlingWinModuleOnCustomHandleException;
        }

        void ExceptionHandlingWinModuleOnCustomHandleException(object sender, CustomHandleExceptionEventArgs customHandleExceptionEventArgs) {
            customHandleExceptionEventArgs.Handled = IsExculded(customHandleExceptionEventArgs.Exception);
        }
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new DisableFiltersNodeUpdater());
            
        }


        bool IsExculded(Exception exception) {
            return (exception is ValidationException) ||
                   (exception.InnerException != null && exception.InnerException is ValidationException);
        }

        protected override void AdditionalViewControlsModuleOnRulesCollected(object sender, EventArgs e) {
            foreach (var typeInfo in XafTypesInfo.Instance.PersistentTypes) {
                var additionalViewControlsRules = LogicRuleManager<IAdditionalViewControlsRule>.Instance[typeInfo];
                foreach (var additionalViewControlsRule in additionalViewControlsRules) {
                    if (additionalViewControlsRule.Id.StartsWith(Module.Captions.Header))
                        additionalViewControlsRule.ControlType = typeof(ApplicationCaption);
                    else if (additionalViewControlsRule.Id.StartsWith(Module.Captions.ViewMessage))
                        additionalViewControlsRule.ControlType = typeof(FooterMessagePanel);
                }
            }
        }

        void ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e) {
            e.AddExtraDiffStore(new WC3LevelMasterDetailModelStore());
            SkinFilterProvider skinFilterProvider = FilterProviderManager.Providers.OfType<SkinFilterProvider>().FirstOrDefault();
            if (skinFilterProvider != null)
                skinFilterProvider.FilterValue = ((IModelApplicationOptionsSkin)Application.Model.Options).Skin;
        }

    }
}
