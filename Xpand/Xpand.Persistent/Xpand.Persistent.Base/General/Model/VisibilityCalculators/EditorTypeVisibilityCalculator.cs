using System;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model.VisibilityCalculators {
    public abstract class EditorTypeVisibilityCalculator<TEditor> : EditorTypeVisibilityCalculator{
        public override bool IsVisible(IModelNode node, string propertyName) {
            return typeof(TEditor).IsAssignableFrom(EditorType(node));
        }
    }

    public abstract class EditorTypeVisibilityCalculator : IModelIsVisible {

        #region Implementation of IModelIsVisible

        protected Type EditorType(IModelNode node){
            var modelNode = node.GetParent<IModelListView>();
            if (modelNode != null) return ((IModelListView) modelNode).EditorType;
            modelNode= node.GetParent<IModelPropertyEditor>();
            return ((IModelPropertyEditor) modelNode).PropertyEditorType;
        }

        #endregion
        #region Implementation of IModelIsVisible
        public abstract bool IsVisible(IModelNode node, string propertyName);
        #endregion
    }
}