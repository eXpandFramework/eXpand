using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General {
    public class MergedDifferencesUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelViews = ((IModelViews)node);
            var objectViews = ObjectViews(modelViews);
            foreach (var modelView in objectViews) {
                var modelNode = ((ModelNode)modelViews[modelView.Id]);
                var layoutStrategy = modelView.MergedDifferences.Any(LayoutStrategy);
                var objectView = (IModelObjectView)modelNode;
                if (layoutStrategy)
                    ClearLayoutNodes(objectView);

                foreach (var mergedDifference in modelView.MergedDifferences) {
                    var modelObjectView = mergedDifference.View;
                    switch (mergedDifference.Strategy) {
                        case MergingStrategy.Everything:
                            modelNode.Merge((ModelNode)modelObjectView);
                            break;
                        case MergingStrategy.OnlyLayout: {
                                MergeOnlyLayout(modelObjectView, modelNode);
                                break;
                            }
                    }
                    if (layoutStrategy)
                        UpdateRemovedNodes(mergedDifference, objectView);
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

        void UpdateRemovedNodes(IModelMergedDifference mergedDifference, IModelObjectView modelObjectView) {
            var modelDetailView = (mergedDifference.View) as IModelDetailView;
            if (modelDetailView != null) {
                var detailView = ((IModelDetailView)modelObjectView);
                UpdateRemovedNodes(modelDetailView.Layout.GetNode(0), detailView.Layout.GetNode(0));
                UpdateRemovedNodes(modelDetailView.Items, detailView.Items);
            } else {
                UpdateRemovedNodes(((IModelListView)mergedDifference.View).Columns, ((IModelListView)modelObjectView).Columns);
            }
        }


        void UpdateRemovedNodes(IModelNode modelNode, IModelNode nodeToUpdate) {
            for (int i = modelNode.NodeCount - 1; i >= 0; i--) {
                var node1 = (ModelNode)modelNode.GetNode(i);
                if (node1.IsRemovedNode) {
                    nodeToUpdate.GetNode(node1.Id).Remove();
                }
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
            var application = ModelApplication(modelViews);
            return application.Application.Views.OfType<IModelObjectViewMergedDifferences>()
                           .Where(view => view.MergedDifferences != null && view.MergedDifferences.Any(difference => IsValid(difference, modelViews.Application.Views)));
        }

        bool IsValid(IModelMergedDifference difference, IModelViews views) {
            if (difference.View == null)
                return false;
            var modelView = views[difference.View.Id] as IModelObjectView;
            return modelView != null && modelView.ModelClass != null;
        }

        ModelApplicationBase ModelApplication(IModelNode modelNode) {
            var node = ((ModelNode)modelNode);
            var modelApplication = node.CreatorInstance.CreateModelApplication();
            foreach (var module in ((IModelSources)node.Application).Modules) {
                var resourcesModelStore = new ResourcesModelStore(module.GetType().Assembly);
                resourcesModelStore.Load(modelApplication);
            }
            return modelApplication;
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