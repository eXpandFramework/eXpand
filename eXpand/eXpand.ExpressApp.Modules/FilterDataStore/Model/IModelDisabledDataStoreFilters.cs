using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.FilterDataStore.NodeGenerators;

namespace eXpand.ExpressApp.FilterDataStore.Model {
    [ModelNodesGenerator(typeof(ModelDisabledDataStoreFiltersNodesGenerator))]
    public interface IModelDisabledDataStoreFilters : IModelNode, IModelList<IModelDisabledDataStoreFilter>
    {
    }
}