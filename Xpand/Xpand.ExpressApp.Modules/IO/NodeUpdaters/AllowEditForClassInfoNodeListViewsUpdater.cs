using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Xpand.ExpressApp.IO.Core;

namespace Xpand.ExpressApp.IO.NodeUpdaters {
    public class AllowEditForClassInfoNodeListViewsUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator>
    {
        public override void UpdateNode(ModelNode node) {
            if (node is IModelListView) {
                var listView = ((IModelListView) node);
                if (listView.ModelClass.TypeInfo.Type == TypesInfo.Instance.ClassInfoGraphNodeType)
                    listView.AllowEdit = true;
            }
        }
    }
}