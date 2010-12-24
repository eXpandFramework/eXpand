using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Xpand.ExpressApp.TreeListEditors.Win.Core;

namespace Xpand.ExpressApp.TreeListEditors {
    public class XpandTreeListEditorServerModeUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelViews = (IModelViews)node;
            foreach (IModelListView modelView in modelViews.OfType<IModelListView>()) {
                modelView.UseServerMode = typeof (ITreeListEditor).IsAssignableFrom(modelView.EditorType);
            }
        }
    }
}