using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using eXpand.ExpressApp.IO.Core;

namespace eXpand.ExpressApp.IO.NodeUpdaters {
    public class AllowEditForClassInfoNodeListViewsUpdater : ModelNodesGeneratorUpdater<ModelListViewNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var listView = ((IModelListView) node);
            if (listView.ModelClass.TypeInfo.Type == TypesInfo.Instance.ClassInfoGraphNodeType)
                listView.AllowEdit = true;
        }
    }
}