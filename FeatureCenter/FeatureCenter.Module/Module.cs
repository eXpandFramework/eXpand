using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl;
using FeatureCenter.Module.ListViewControl.PropertyPathFilters;
using FeatureCenter.Module.LowLevelFilterDataStore;
using FeatureCenter.Module.WorldCreator;
using FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail;
using Xpand.ExpressApp;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation;
using Xpand.ExpressApp.ModelDifference;
using CreateCustomModelDifferenceStoreEventArgs = Xpand.ExpressApp.ModelDifference.CreateCustomModelDifferenceStoreEventArgs;


namespace FeatureCenter.Module {
    public sealed partial class FeatureCenterModule : XpandModuleBase {
        static XafApplication _application;

        public FeatureCenterModule() {
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(Analysis)), IsExportedType));
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(Xpand.Persistent.BaseImpl.Updater)), IsExportedType));
//            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(SecurityUser)), IsExportedType));
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(ThresholdSeverity)), IsExportedType));

            InitializeComponent();

        }

        void ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs createCustomModelDifferenceStoreEventArgs) {
            createCustomModelDifferenceStoreEventArgs.AddExtraDiffStore(new ExistentAssemblyMasterDetailModelStore());
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);

            var typeInfos = typesInfo.PersistentTypes.Where(info => info.FindAttribute<WhatsNewAttribute>() != null).OrderBy(typeInfo => typeInfo.FindAttribute<WhatsNewAttribute>().Date);
            foreach (var typeInfo in typeInfos) {
                var whatsNewAttribute = typeInfo.FindAttribute<WhatsNewAttribute>();
                var xpandNavigationItemAttribute = whatsNewAttribute.XpandNavigationItemAttribute;
                typeInfo.AddAttribute(new XpandNavigationItemAttribute("Whats New/" + xpandNavigationItemAttribute.Path, xpandNavigationItemAttribute.ViewId, xpandNavigationItemAttribute.ObjectKey));
            }

        }

        public new static XafApplication Application {
            get { return _application; }
        }
        protected override void OnApplicationInitialized(XafApplication xafApplication) {
            base.OnApplicationInitialized(xafApplication);
            _application = xafApplication;
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);

            var modelDifferenceBaseModule = (ModelDifferenceBaseModule)moduleManager.Modules.Where(
                    mbase => typeof(ModelDifferenceBaseModule).IsAssignableFrom(mbase.GetType())).SingleOrDefault();
            if (modelDifferenceBaseModule != null)
                modelDifferenceBaseModule.CreateCustomModelDifferenceStore += ModelDifferenceBaseModuleOnCreateCustomModelDifferenceStore;
        }


        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new PropertyPathFiltersNodeUpdater(Application));
            updaters.Add(new DisableFiltersNodeUpdater());
            updaters.Add(new ModelSystemTablesUpdater());
        }
    }
}
