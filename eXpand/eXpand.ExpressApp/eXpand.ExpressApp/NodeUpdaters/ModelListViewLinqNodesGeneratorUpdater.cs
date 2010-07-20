using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.NodeUpdaters {
    public class ModelListViewLinqNodesGeneratorUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator>
    {
        public override void UpdateNode(ModelNode node)
        {
            foreach (IModelClass classInfo in node.Application.BOModel)
            {
                if (classInfo.TypeInfo.IsPersistent)
                {
                    if (!string.IsNullOrEmpty(classInfo.Name))
                    {
                        foreach (string method in LinqCollectionSourceHelper.GetXPQueryMethods(classInfo.TypeInfo.Type))
                        {
                            string id = ModelListViewNodesGenerator.GetListViewId(classInfo.TypeInfo.Name) + "_" + method + LinqCollectionSource.DefaultSuffix;
                            var listViewInfo = node.Application.Views.GetNode<IModelListView>(id) ??
                                               node.AddNode<IModelListView>(id);
                            listViewInfo.ModelClass = classInfo;
                            ((IModelListViewLinq)listViewInfo).XPQueryMethod = method;
                        }
                    }
                }
            }
        }
    }
}