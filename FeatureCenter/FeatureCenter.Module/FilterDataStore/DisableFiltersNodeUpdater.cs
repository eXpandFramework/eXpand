using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.FilterDataStore.Core;
using eXpand.ExpressApp.FilterDataStore.Model;
using eXpand.ExpressApp.FilterDataStore.NodeGenerators;
using FeatureCenter.Module.FilterDataStore.ContinentFilter;
using FeatureCenter.Module.FilterDataStore.SkinFilter;
using FeatureCenter.Module.FilterDataStore.UserFilter;

namespace FeatureCenter.Module.FilterDataStore {
    public class DisableFiltersNodeUpdater : ModelNodesGeneratorUpdater<ModelDisabledDataStoreFiltersNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            foreach (FilterProviderBase provider in FilterProviderManager.Providers) {
                if (((IModelClass) node.Parent).TypeInfo.Type==typeof(FDSCCustomer)&&provider.Name=="ContinentFilterProvider")
                    continue;
                if (((IModelClass)node.Parent).TypeInfo.Type == typeof(FDSSCustomer) && provider.Name == "SkinFilterProvider")
                    continue;
                if (((IModelClass)node.Parent).TypeInfo.Type == typeof(FDSUCustomer) && provider.Name == "UserFilterProvider")
                    continue;
                var modelDisabledDataStoreFilter = node.AddNode<IModelDisabledDataStoreFilter>();
                modelDisabledDataStoreFilter.Name = provider.Name;
            }
        }
    }
}