using System;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model.VisibilityCalculators {
    public abstract class EditorTypeVisibilityCalculator<TEditor,TParentNode> : EditorTypeVisibilityCalculator<TParentNode> where TParentNode : class, IModelNode{
        public override bool IsVisible(IModelNode node, string propertyName) {
            return typeof(TEditor).IsAssignableFrom(EditorType(node));
        }
    }

    public abstract class EditorTypeVisibilityCalculator<TParentNode> : IModelIsVisible where TParentNode: class, IModelNode{

        #region Implementation of IModelIsVisible

        protected Type EditorType(IModelNode node){
            var modelNode = node.GetParent<TParentNode>();
            var modelListView = modelNode as IModelListView;
            if (modelListView != null) 
                return modelListView.EditorType;
            var modelColumn = modelNode as IModelColumn;
            if (modelColumn != null) return modelColumn.PropertyEditorType;
            var modelPropertyEditor = modelNode as IModelPropertyEditor;
            if (modelPropertyEditor != null) return modelPropertyEditor.PropertyEditorType;
            throw new NotImplementedException(GetType().FullName);
        }

        #endregion
        #region Implementation of IModelIsVisible
        public abstract bool IsVisible(IModelNode node, string propertyName);
        #endregion
    }
}