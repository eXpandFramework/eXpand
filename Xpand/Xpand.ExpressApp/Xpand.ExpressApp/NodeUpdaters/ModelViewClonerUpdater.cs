using System;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Xpand.ExpressApp.Attributes;

namespace Xpand.ExpressApp.NodeUpdaters {
    public class ModelViewClonerUpdater:ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelClasses = node.Application.BOModel.Where(modelClass => modelClass.TypeInfo.FindAttribute<CloneViewAttribute>()!=null);
            
            foreach (var modelClass in modelClasses){
                var cloneViewAttributes = modelClass.TypeInfo.FindAttributes<CloneViewAttribute>().OrderBy(viewAttribute => viewAttribute.ViewType);
                foreach (var cloneViewAttribute in cloneViewAttributes) {
                    IModelView modelView = GetModelView(modelClass, cloneViewAttribute);
                    ModelNode cloneNodeFrom = ((ModelNode) node.Application.Views).CloneNodeFrom((ModelNode) modelView,cloneViewAttribute.ViewId);
                    if (modelView is IModelListView&& !(string.IsNullOrEmpty(cloneViewAttribute.DetailView))) {
                        CloneViewAttribute attribute = cloneViewAttribute;
                        var modelDetailView = node.Application.Views.OfType<IModelDetailView>().Where(view => view.Id == attribute.DetailView).FirstOrDefault();
                        if (modelDetailView== null)
                            throw new NullReferenceException(attribute.DetailView);
                        ((IModelListView)cloneNodeFrom).DetailView = modelDetailView;
                    }
                }
            }
        }

        IModelView GetModelView(IModelClass modelClass, CloneViewAttribute cloneViewAttribute) {
            if (cloneViewAttribute.ViewType==CloneViewType.LookupListView)
                return modelClass.DefaultLookupListView;
            if (cloneViewAttribute.ViewType == CloneViewType.DetailView)
                return modelClass.DefaultDetailView;
            return modelClass.DefaultListView;
        }
    }
}