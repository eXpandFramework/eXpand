using System;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model.VisibilityCalculators {
    public class EditorTypeVisibilityCalculator<TEditor,TParentNode> : EditorTypeVisibilityCalculator<TParentNode> where TParentNode : class, IModelNode{
        public override bool IsVisible(IModelNode node, string propertyName) {
            return typeof(TEditor).IsAssignableFrom(EditorType(node));
        }
    }

    public abstract class EditorTypeVisibilityCalculator<TParentNode> : IModelIsVisible where TParentNode: class, IModelNode{

        #region Implementation of IModelIsVisible

        protected Type EditorType(IModelNode node){
            var modelNode = node.GetParent<TParentNode>();
            if (modelNode is IModelListView modelListView) 
                return modelListView.EditorType;
            if (modelNode is IModelMemberViewItem memberViewItem) return memberViewItem.PropertyEditorType;
            throw new NotImplementedException(GetType().FullName);
        }

        #endregion
        #region Implementation of IModelIsVisible
        public abstract bool IsVisible(IModelNode node, string propertyName);
        #endregion
    }
}