using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Xpand.ExpressApp.TreeListEditors.Win.ListEditors;

namespace Xpand.ExpressApp.TreeListEditors.Win.Core {
    public class XpandTreeListEditorNodeGeneratorUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelViews = (IModelViews)node;
            foreach (IModelListView modelView in modelViews.OfType<IModelListView>()) {
                if (modelView.EditorType == typeof(XpandTreeListEditor))
                    modelView.UseServerMode = false;
            }
        }
    }
}