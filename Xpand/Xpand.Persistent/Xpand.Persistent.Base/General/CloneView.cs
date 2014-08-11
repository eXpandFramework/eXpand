using System;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

namespace Xpand.Persistent.Base.General {
    public class ModelViewClonerUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelClasses = node.Application.BOModel.Where(modelClass => modelClass.TypeInfo.FindAttribute<CloneViewAttribute>() != null);

            foreach (var modelClass in modelClasses) {
                var cloneViewAttributes = modelClass.TypeInfo.FindAttributes<CloneViewAttribute>(false).OrderBy(viewAttribute => viewAttribute.ViewType);
                foreach (var cloneViewAttribute in cloneViewAttributes) {
                    if (node.Application.Views[cloneViewAttribute.ViewId]==null) {
                        var modelView = GetModelView(modelClass, cloneViewAttribute);
                        var cloneNodeFrom = ((ModelNode) modelView).Clone(cloneViewAttribute.ViewId);
                        AssignAsDefaultView(cloneViewAttribute, (IModelObjectView) cloneNodeFrom);
                        if (modelView is IModelListView && !(string.IsNullOrEmpty(cloneViewAttribute.DetailView))) {
                            CloneViewAttribute attribute = cloneViewAttribute;
                            var modelDetailView =node.Application.Views.OfType<IModelDetailView>().FirstOrDefault(view 
                                => view.Id == attribute.DetailView);
                            if (modelDetailView == null)
                                throw new NullReferenceException(attribute.DetailView);
                            ((IModelListView) cloneNodeFrom).DetailView = modelDetailView;
                        }
                    }
                }
            }
        }

        void AssignAsDefaultView(CloneViewAttribute cloneViewAttribute, IModelObjectView modelView) {
            if (cloneViewAttribute.IsDefault) {
                var view = modelView as IModelListView;
                if (view != null) {
                    view.ModelClass.DefaultListView = view;
                }
                else {
                    modelView.ModelClass.DefaultDetailView = (IModelDetailView) modelView;
                }
            }
        }

        IModelObjectView GetModelView(IModelClass modelClass, CloneViewAttribute cloneViewAttribute) {
            if (cloneViewAttribute.ViewType == CloneViewType.LookupListView)
                return modelClass.DefaultLookupListView;
            if (cloneViewAttribute.ViewType == CloneViewType.DetailView)
                return modelClass.DefaultDetailView;
            return modelClass.DefaultListView;
        }
    }
    public enum CloneViewType {
        DetailView,
        ListView,
        LookupListView
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class CloneViewAttribute : Attribute, ISupportViewId {
        readonly CloneViewType _viewType;
        readonly string _viewId;
        readonly bool _isDefault;

        public CloneViewAttribute(CloneViewType viewType, string viewId, bool isDefault = false) {
            _viewType = viewType;
            _viewId = viewId;
            _isDefault = isDefault;
        }

        public bool IsDefault {
            get { return _isDefault; }
        }

        public string ViewId {
            get { return _viewId; }
        }


        public CloneViewType ViewType {
            get { return _viewType; }
        }

        public string DetailView { get; set; }
    }
}