using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using FeatureCenter.Module.Win.ApplicationDifferences.ExternalApplication;
using FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyCalculatedField;
using FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyMasterDetail;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;

using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.ModelDifference;
using Xpand.ExpressApp.WorldCreator.System;
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
            var modelDifferenceBaseModule = (ModelDifferenceBaseModule)moduleManager.Modules.FirstOrDefault(mbase => mbase is ModelDifferenceBaseModule);
            if (modelDifferenceBaseModule != null)
                modelDifferenceBaseModule.CreateCustomModelDifferenceStore += ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore;
        }

        protected override void AdditionalViewControlsModuleOnRulesCollected(object sender, EventArgs e) {
            foreach (var typeInfo in XafTypesInfo.Instance.PersistentTypes) {
                var additionalViewControlsRules = LogicRuleManager.Instance[typeInfo].OfType<IAdditionalViewControlsRule>();
                foreach (var additionalViewControlsRule in additionalViewControlsRules) {
                    if (additionalViewControlsRule.Id.StartsWith(Module.Captions.Header))
                        additionalViewControlsRule.ControlType = typeof(ApplicationCaption);
                    else if (additionalViewControlsRule.Id.StartsWith(Module.Captions.ViewMessage))
                        additionalViewControlsRule.ControlType = typeof(FooterMessagePanel);
                }
            }
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return base.GetModuleUpdaters(objectSpace, versionFromDB).Where(updater => !(updater is WorldCreatorModuleUpdater));
        }

        void ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e) {

            e.AddExtraDiffStore(new WCCalculatedFieldModelStore());
            e.AddExtraDiffStore(new WC3LevelMasterDetailModelStore());
        }

    }
}
