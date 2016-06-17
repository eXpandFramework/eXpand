using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.FilterDataStore.NodeGenerators;

namespace Xpand.ExpressApp.FilterDataStore.Model {
    [ModelNodesGenerator(typeof(ModelSystemTablesNodesGenerator))]
    public interface IModelFilterDataStoreSystemTables : IModelNode, IModelList<IModelFilterDataStoreSystemTable>
    {
    }
}