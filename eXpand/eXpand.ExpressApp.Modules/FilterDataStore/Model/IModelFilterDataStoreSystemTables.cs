using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.FilterDataStore.NodeGenerators;

namespace eXpand.ExpressApp.FilterDataStore.Model {
    [ModelNodesGenerator(typeof(ModelSystemTablesNodesGenerator))]
    public interface IModelFilterDataStoreSystemTables : IModelNode, IModelList<IModelFilterDataStoreSystemTable>
    {
    }
}