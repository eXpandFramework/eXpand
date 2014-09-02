using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using FeatureCenter.Module.LowLevelFilterDataStore.ContinentFilter;
using FeatureCenter.Module.LowLevelFilterDataStore.UserFilter;
using Xpand.ExpressApp.FilterDataStore.Core;
using Xpand.ExpressApp.FilterDataStore.Model;
using Xpand.ExpressApp.FilterDataStore.NodeGenerators;

namespace FeatureCenter.Module.LowLevelFilterDataStore {
    public class DisableFiltersNodeUpdater : ModelNodesGeneratorUpdater<ModelDisabledDataStoreFiltersNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            if (FilterProviderManager.IsRegistered)
                foreach (FilterProviderBase provider in FilterProviderManager.Providers) {
                    if (((IModelClass)node.Parent).TypeInfo.Type == typeof(FDSCCustomer) && provider.Name == "ContinentFilterProvider")
                        continue;
                    if (((IModelClass)node.Parent).TypeInfo.Type == typeof(FDSUCustomer) && provider.Name == "UserFilterProvider")
                        continue;
                    if (node[provider.Name]==null){
                        var modelDisabledDataStoreFilter = node.AddNode<IModelDisabledDataStoreFilter>();
                        modelDisabledDataStoreFilter.Name = provider.Name;
                    }
                }
        }
    }
}