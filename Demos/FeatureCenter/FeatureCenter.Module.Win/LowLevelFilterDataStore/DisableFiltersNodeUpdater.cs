using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using FeatureCenter.Module.Win.LowLevelFilterDataStore.SkinFilter;
using Xpand.ExpressApp.FilterDataStore.NodeGenerators;

namespace FeatureCenter.Module.Win.LowLevelFilterDataStore {
    public class DisableFiltersNodeUpdater : ModelNodesGeneratorUpdater<ModelDisabledDataStoreFiltersNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            if (((IModelClass)node.Parent).TypeInfo.Type == typeof(FDSSCustomer)) {
                IModelNode modelNode = node.GetNode("SkinFilterProvider");
                if (modelNode != null) modelNode.Remove();
            }
        }
    }
}