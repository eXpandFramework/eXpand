using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using FeatureCenter.Module.Win.ApplicationDifferences.ExternalApplication;
using Xpand.ExpressApp.AdditionalViewControlsProvider;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;
using Xpand.ExpressApp.ExceptionHandling.Win;
using Xpand.ExpressApp.FilterDataStore.Core;
using Xpand.ExpressApp.FilterDataStore.Win.Providers;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.ModelDifference;
using CreateCustomModelDifferenceStoreEventArgs = Xpand.ExpressApp.ModelDifference.CreateCustomModelDifferenceStoreEventArgs;

namespace FeatureCenter.Module.Win
{
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class FeatureCenterWindowsFormsModule : ModuleBase
    {
        public FeatureCenterWindowsFormsModule()
        {
            InitializeComponent();
            ParametersFactory.RegisterParameter(new ExternalApplicationKeyParameter());
        }
        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            var modelDifferenceBaseModule = (ModelDifferenceBaseModule)moduleManager.Modules.Where(
                    mbase => typeof(ModelDifferenceBaseModule).IsAssignableFrom(mbase.GetType())).Single();
            modelDifferenceBaseModule.CreateCustomModelDifferenceStore += ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore;
            var additionalViewControlsModule = (AdditionalViewControlsModule)moduleManager.Modules.FindModule(typeof(AdditionalViewControlsModule));
            additionalViewControlsModule.RulesCollected += AdditionalViewControlsModuleOnRulesCollected;
            var exceptionHandlingWinModule =
                (ExceptionHandlingWinModule) moduleManager.Modules.FindModule(typeof (ExceptionHandlingWinModule));
            exceptionHandlingWinModule.CustomHandleException+=ExceptionHandlingWinModuleOnCustomHandleException;            

        }

        void ExceptionHandlingWinModuleOnCustomHandleException(object sender, CustomHandleExceptionEventArgs customHandleExceptionEventArgs) {
            customHandleExceptionEventArgs.Handled = IsExculded(customHandleExceptionEventArgs.Exception);
        }

        bool IsExculded(Exception exception)
        {
            return (exception is ValidationException) ||
                   (exception.InnerException != null && exception.InnerException is ValidationException);
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
