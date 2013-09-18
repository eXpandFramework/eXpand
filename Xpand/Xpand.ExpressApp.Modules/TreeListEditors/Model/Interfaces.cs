using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using System.Linq;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Fasterflect;

namespace Xpand.ExpressApp.TreeListEditors.Model {
    [ModelAbstractClass]
    public interface IModelListViewOptionsTreeList : IModelListViewOptionsColumnView {
        [ModelBrowsable(typeof(TreeListEditorVisibilityCalculator))]
        IModelOptionsTreeList TreeListOptions { get; }
    }
    [ModelAbstractClass]
    public interface IModelListViewOptionsTreeListNavigation : IModelRootNavigationItems {
        IModelOptionsTreeList TreeListOptions { get; }
    }
    public interface IModelOptionsTreeList : IModelOptionsColumnView {

    }
    public class TreeListEditorVisibilityCalculator : EditorTypeVisibilityCalculator {
        #region Overrides of EditorTypeVisibilityCalculator
        public override bool IsVisible(IModelNode node, string propertyName) {
            var treeListEditorType = TreeListEditorType();
            return treeListEditorType != null && treeListEditorType.IsAssignableFrom(EditorType(node));
        }

        protected virtual Type TreeListEditorType() {
            var typeInfo = ReflectionHelper.FindTypeDescendants(XafTypesInfo.CastTypeToTypeInfo(typeof(TreeListEditorVisibilityCalculatorHelper))).SingleOrDefault();
            if (typeInfo != null) {
                var visibilityCalculatorHelper = (TreeListEditorVisibilityCalculatorHelper)typeInfo.Type.CreateInstance();
                return visibilityCalculatorHelper.TreelistEditorType();
            }
            return null;
        }
        #endregion
    }

    public abstract class TreeListEditorVisibilityCalculatorHelper {
        public abstract Type TreelistEditorType();
    }
    [ModelAbstractClass]
    public interface IModelColumnOptionsTreeListView : IModelColumnOptionsColumnView {
        [ModelBrowsable(typeof(TreeListEditorVisibilityCalculator))]
        IModelOptionsColumnTreeListView TreeListColumnOptions { get; }
    }
    public interface IModelOptionsColumnTreeListView : IModelColumnViewColumnOptions {
    }

}
