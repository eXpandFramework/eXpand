using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General {
    public class MergedDifferencesUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelViews = ((IModelViews)node);
            IEnumerable<IModelObjectViewMergedDifferences> objectViews = ObjectViews(modelViews);

            foreach (var modelView in objectViews) {
                Merge(modelViews, modelView);
            }
        }

        void Merge(IModelViews modelViews, IModelObjectViewMergedDifferences modelView) {
            var modelNode = ((IModelNode) modelViews[modelView.Id]);
            var layoutStrategy = modelView.MergedDifferences.Any(LayoutStrategy);
            var objectView = (IModelObjectView) modelNode;
            if (layoutStrategy) {
                ClearLayoutNodes(objectView);
            }

            foreach (var mergedDifference in MergedDifferences(modelView)) {
                var modelObjectView = mergedDifference.View;
                switch (mergedDifference.Strategy) {
                    case MergingStrategy.Everything: {
                        new ModelXmlReader().ReadFromString(modelNode, "", PrepareXml(modelNode, modelObjectView));
                        break;
                    }
                    case MergingStrategy.OnlyLayout: {
                            MergeOnlyLayout(modelObjectView, (ModelNode) modelNode);
                        break;
                    }
                }
                if (layoutStrategy)
                    UpdateRemovedNodes(mergedDifference, objectView);
            }
        }

        IEnumerable<IModelMergedDifference> MergedDifferences(IModelObjectViewMergedDifferences modelView) {
            var modelMergedDifferences = new List<IModelMergedDifference>();
            foreach (var modelMergedDifference in modelView.MergedDifferences) {
                var modelObjectViewMergedDifferences = (IModelObjectViewMergedDifferences) modelMergedDifference.View;
                if (modelObjectViewMergedDifferences.MergedDifferences!=null)
                    modelMergedDifferences.AddRange(MergedDifferences(modelObjectViewMergedDifferences));
            }
            modelMergedDifferences.AddRange(modelView.MergedDifferences);
            return modelMergedDifferences;
        }

        string PrepareXml(IModelNode modelNode, IModelObjectView modelObjectView) {
            var xml = ((ModelNode) modelObjectView).Xml;
            xml= Regex.Replace(xml, "(<DetailView Id=\")([^\"]*)\"", "$1" + modelNode.GetValue<string>("Id") + "\"", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return Regex.Replace(xml, "<MergedDifferences[^>]*>(.*?)</MergedDifferences>", "",RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        void UpdateRemovedNodes(IModelMergedDifference mergedDifference, IModelObjectView modelObjectView) {
            var modelDetailView = (mergedDifference.View) as IModelDetailView;
            if (modelDetailView != null) {
                var detailView = ((IModelDetailView)modelObjectView);
                if (modelDetailView.Layout != null)
                    UpdateRemovedNodes(modelDetailView.Layout, detailView.Layout);
                if (modelDetailView.Items != null) 
                    UpdateRemovedNodes(modelDetailView.Items, detailView.Items);
            } else {
                UpdateRemovedNodes(((IModelListView)mergedDifference.View).Columns, ((IModelListView)modelObjectView).Columns);
            }
        }

        void UpdateRemovedNodes(IModelNode modelNode, IModelNode nodeToUpdate) {
            for (int i = modelNode.NodeCount - 1; i >= 0; i--) {
                var node = modelNode.GetNode(i);
                var modelNode1 = nodeToUpdate.GetNode(i);
                if (modelNode1!=null) {
                    if (((ModelNode) node).IsRemovedNode) {
                        modelNode1.Remove();
                    }
                    UpdateRemovedNodes(node, modelNode1);
                }
            }
        }

        void MergeOnlyLayout(IModelObjectView modelObjectView, ModelNode modelNode) {
            var modelDetailView = modelObjectView as IModelDetailView;
            if (modelDetailView != null) {
                var detailView = ((IModelDetailView)modelNode);
                ((ModelNode)detailView.Items).Merge((ModelNode)modelDetailView.Items);
                ((ModelNode)detailView.Layout).Merge((ModelNode)modelDetailView.Layout);
            } else {
                ((ModelNode)((IModelListView)modelNode).Columns).Merge((ModelNode)((IModelListView)modelObjectView).Columns);
            }
        }


        bool LayoutStrategy(IModelMergedDifference difference) {
            return difference.Strategy == MergingStrategy.Everything || difference.Strategy == MergingStrategy.OnlyLayout;
        }

        void ClearLayoutNodes(IModelObjectView modelObjectView) {
            var modelDetailView = modelObjectView as IModelDetailView;
            if (modelDetailView != null) {
                var modelNode = modelDetailView.Layout.GetNode(0);
                for (int i = modelNode.NodeCount - 1; i >= 0; i--) {
                    modelNode.GetNode(i).Remove();
                }
            }
        }

        IEnumerable<IModelObjectViewMergedDifferences> ObjectViews(IModelViews modelViews) {
            var application = ((IModelApplicationResourceDifferences) modelViews.Application).ResourceDifferencesApplication;
            return application.Application.Views.OfType<IModelObjectViewMergedDifferences>()
                           .Where(view => view.MergedDifferences != null && view.MergedDifferences.Any(difference => IsValid(difference, modelViews.Application.Views))).ToList();
        }

        bool IsValid(IModelMergedDifference difference, IModelViews views) {
            if (difference.View == null)
                return false;
            var modelView = views[difference.View.Id] as IModelObjectView;
            return modelView != null && modelView.ModelClass != null;
        }
    }

    public enum MergingStrategy {
        Everything,
        OnlyLayout
    }
    [ModelAbstractClass]
    public interface IModelObjectViewMergedDifferences : IModelView {
        IModelMergedDifferences MergedDifferences { get; }
    }

    public interface IModelMergedDifferences : IModelNode, IModelList<IModelMergedDifference> {

    }

    public interface IModelMergedDifference : IModelNode {
        [DataSourceProperty("Views")]
        IModelObjectView View { get; set; }
        [Browsable(false)]
        IModelList<IModelObjectView> Views { get; }

        MergingStrategy Strategy { get; set; }
    }
    [DomainLogic(typeof(IModelMergedDifference))]
    public class ModelMergedDViifferenceDomainLogic {

        public static IModelList<IModelObjectView> Get_Views(IModelMergedDifference differences) {
            var modelObjectView = ((IModelObjectView)differences.Parent.Parent);
            if (modelObjectView.ModelClass == null)
                return new CalculatedModelNodeList<IModelObjectView>(differences.Application.Views.OfType<IModelObjectView>());
            var modelObjectViews = differences.Application.Views.OfType<IModelObjectView>().Where(view
                => view.ModelClass != null && (view.ModelClass.TypeInfo.Type.IsAssignableFrom(modelObjectView.ModelClass.TypeInfo.Type)
                && (modelObjectView is IModelListView ? view is IModelListView : view is IModelDetailView)));
            return new CalculatedModelNodeList<IModelObjectView>(modelObjectViews);
        }

    }
}