using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.ModelDifference;
using FeatureCenter.Module.FilterDataStore;
using FeatureCenter.Module.IO.DynamicAssemblyMasterDetail;
using FeatureCenter.Module.ModelDifference.ExternalApplication;
using FeatureCenter.Module.PropertyPath;
using System.Linq;
using FeatureCenter.Module.WorldCreator.DynamicAssemblyMasterDetail;
using FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail;
using CreateCustomModelDifferenceStoreEventArgs = eXpand.ExpressApp.ModelDifference.CreateCustomModelDifferenceStoreEventArgs;
using ModuleBase = eXpand.ExpressApp.ModuleBase;


namespace FeatureCenter.Module
{
    public sealed partial class FeatureCenterModule : ModuleBase
    {
        public FeatureCenterModule()
        {
            InitializeComponent();
            ParametersFactory.RegisterParameter(new ExternalApplicationKeyParameter());
        }

        void ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs createCustomModelDifferenceStoreEventArgs) {
            createCustomModelDifferenceStoreEventArgs.AddExtraDiffStore(new ExistentAssemblyMasterDetailModelStore());
            createCustomModelDifferenceStoreEventArgs.AddExtraDiffStore(new WC3LevelMasterDetailModelStore());
            createCustomModelDifferenceStoreEventArgs.AddExtraDiffStore(new IOWC3LevelMasterDetailModelStore());
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
