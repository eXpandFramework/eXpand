using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace Xpand.ExpressApp.ModelDifference.Win {
    internal class AsyncLodingUpdater:ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var mdoViews = ((IModelViews) node).OfType<IModelDetailView>().Where(view => typeof(ModelDifferenceObject).IsAssignableFrom(view.ModelClass.TypeInfo.Type));
            foreach (var modelObjectView in mdoViews.Cast<IModelAsync>()) {
                modelObjectView.UseAsyncLoading = false;
            }
        }
    }
}
