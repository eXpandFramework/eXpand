using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class CollectionsEditModeController : ViewEditModeController{
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.ViewChanging+=FrameOnViewChanging;
        }


        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var detailView = View;
            foreach (ListPropertyEditor listPropertyEditor in detailView.GetItems<ListPropertyEditor>()) {
                var control = listPropertyEditor.Control as System.Web.UI.Control;
                if (control != null) {
                    control.Visible = GetViewEditMode(listPropertyEditor) || (detailView.ViewEditMode == ViewEditMode.View);
                }
            }
        }

        bool GetViewEditMode(ListPropertyEditor listPropertyEditor) {
            var modelPropertyEditor = ((IModelPropertyEditor)listPropertyEditor.Model);
            var viewEditMode = ((IModelViewEditMode) modelPropertyEditor.View).ViewEditMode;
            return viewEditMode.HasValue && viewEditMode.Value == ViewEditMode.Edit;
        }

        void FrameOnViewChanging(object sender, ViewChangingEventArgs viewChangingEventArgs) {
            var viewEditMode = ((IModelViewEditMode) viewChangingEventArgs.View.Model).ViewEditMode;
            if (viewEditMode.HasValue&&viewChangingEventArgs.View is ListView) {
                var showViewStrategy = ((ShowViewStrategy)Application.ShowViewStrategy);
                showViewStrategy.CollectionsEditMode = viewEditMode.Value;
            }
        }

    }
}
