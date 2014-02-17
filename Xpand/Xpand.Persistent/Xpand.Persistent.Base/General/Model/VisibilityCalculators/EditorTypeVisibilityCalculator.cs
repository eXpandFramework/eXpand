using System;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.General.Model.VisibilityCalculators {
    public abstract class EditorTypeVisibilityCalculator : IModelIsVisible {

        #region Implementation of IModelIsVisible

        protected Type EditorType(IModelNode node) {
            return ((IModelListView) node.GetParent<IModelListView>()).EditorType;
        }
        #endregion
        #region Implementation of IModelIsVisible
        public abstract bool IsVisible(IModelNode node, string propertyName);
        #endregion
    }
}