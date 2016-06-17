using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.FilterDataStore.NodeGenerators;

namespace Xpand.ExpressApp.FilterDataStore.Model {
    [ModelNodesGenerator(typeof(ModelDisabledDataStoreFiltersNodesGenerator))]
    public interface IModelDisabledDataStoreFilters : IModelNode, IModelList<IModelDisabledDataStoreFilter>
    {
    }
}