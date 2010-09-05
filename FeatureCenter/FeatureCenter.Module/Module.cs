using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp;
using Xpand.ExpressApp.ModelDifference;
using FeatureCenter.Module.ApplicationDifferences.ExternalApplication;
using System.Linq;
using FeatureCenter.Module.ListViewControl.PropertyPathFilters;
using FeatureCenter.Module.LowLevelFilterDataStore;
using FeatureCenter.Module.WorldCreator.DynamicAssemblyMasterDetail;
using FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail;
using CreateCustomModelDifferenceStoreEventArgs = Xpand.ExpressApp.ModelDifference.CreateCustomModelDifferenceStoreEventArgs;


namespace FeatureCenter.Module
{
    public sealed partial class FeatureCenterModule : XpandModuleBase
    {
        public FeatureCenterModule()
        {
            InitializeComponent();
            ParametersFactory.RegisterParameter(new ExternalApplicationKeyParameter());
        }

        void ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs createCustomModelDifferenceStoreEventArgs) {
            createCustomModelDifferenceStoreEventArgs.AddExtraDiffStore(new ExistentAssemblyMasterDetailModelStore());
            createCustomModelDifferenceStoreEventArgs.AddExtraDiffStore(new WC3LevelMasterDetailModelStore());
        }


        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            
            var modelDifferenceBaseModule =(ModelDifferenceBaseModule)moduleManager.Modules.Where(
                    mbase => typeof (ModelDifferenceBaseModule).IsAssignableFrom(mbase.GetType())).SingleOrDefault();
            if (modelDifferenceBaseModule != null)
                modelDifferenceBaseModule.CreateCustomModelDifferenceStore +=ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore;
        }


        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters){
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new PropertyPathFiltersNodeUpdater(Application));
            updaters.Add(new DisableFiltersNodeUpdater());
        }
    }
}
