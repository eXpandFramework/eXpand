using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.ExpressApp.IO.NodeUpdaters{
    public class AllowEditForClassInfoNodeListViewsUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator>{
        public override void UpdateNode(ModelNode node){
            var view = node as IModelListView;
            if (view != null){
                var listView = view;
                listView.AllowEdit = typeof(IClassInfoGraphNode).IsAssignableFrom(listView.ModelClass.TypeInfo.Type);
            }
        }
    }
}