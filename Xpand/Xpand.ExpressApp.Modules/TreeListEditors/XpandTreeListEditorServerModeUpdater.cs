using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Persistent.Base;
using Fasterflect;
using Xpand.ExpressApp.TreeListEditors.Model;

namespace Xpand.ExpressApp.TreeListEditors {
    public class XpandTreeListEditorServerModeUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modelViews = (IModelViews)node;
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(TreeListEditorVisibilityCalculatorHelper));
            var firstOrDefault = ReflectionHelper.FindTypeDescendants(typeInfo).FirstOrDefault(info=> !info.IsAbstract);
            if (firstOrDefault != null){
                var type = firstOrDefault.Type;
                var helper=(TreeListEditorVisibilityCalculatorHelper) type.CreateInstance();
                var modelListViews = modelViews.OfType<IModelListView>().Where(view => helper.TreelistEditorType().Any(type1 
                    => type1.IsAssignableFrom(view.EditorType)));
                foreach (var modelListView in modelListViews) {
                    modelListView.UseServerMode = false;
                }
            }
        }
    }
}