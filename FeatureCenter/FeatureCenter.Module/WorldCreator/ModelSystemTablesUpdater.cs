using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.FilterDataStore.Model;
using Xpand.ExpressApp.FilterDataStore.NodeGenerators;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using System.Linq;

namespace FeatureCenter.Module.WorldCreator {
    public class ModelSystemTablesUpdater : ModelNodesGeneratorUpdater<ModelSystemTablesNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var typeInfos = XafTypesInfo.Instance.PersistentTypes.Where(info => {
                var startsWith = (info.Type.Namespace+"").StartsWith(typeof(PersistentClassInfo).Namespace+"");
                return startsWith;
            });
            foreach (var typeInfo in typeInfos) {
                node.AddNode<IModelFilterDataStoreSystemTable>(typeInfo.Type.Name);    
            }
        }
    }
}
