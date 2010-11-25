using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Model;

namespace Xpand.ExpressApp.SystemModule {
    public class ListViewViewModeDetailViewController : ViewController<ListView> {
        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<ListViewProcessCurrentObjectController>().CustomizeShowViewParameters += OnCustomizeShowViewParameters;
        }
        protected override void OnDeactivating() {
            base.OnDeactivating();
            Frame.GetController<ListViewProcessCurrentObjectController>().CustomizeShowViewParameters -= OnCustomizeShowViewParameters;
        }
        void OnCustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs customizeShowViewParametersEventArgs) {
            var showViewParameters = customizeShowViewParametersEventArgs.ShowViewParameters;
            var createdView = showViewParameters.CreatedView as DetailView;
            if (createdView != null && createdView.ViewEditMode == ViewEditMode.View) {
                var viewModeDetailView = ((IModelListViewViewModeDetailView)View.Model).ViewModeDetailView;
                if (View.Model.DetailView != viewModeDetailView)
                    createdView.SetInfo(viewModeDetailView);
            }
        }
    }
}
