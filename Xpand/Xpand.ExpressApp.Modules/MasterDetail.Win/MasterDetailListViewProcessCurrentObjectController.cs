using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;

namespace eXpand.ExpressApp.MasterDetail.Win {
    public class MasterDetailListViewProcessCurrentObjectController : MasterDetailBaseController
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            return;
            var listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
            listViewProcessCurrentObjectController.CustomizeShowViewParameters+=OnCustomizeShowViewParameters;
            listViewProcessCurrentObjectController.CustomProcessSelectedItem+=ListViewProcessCurrentObjectControllerOnCustomProcessSelectedItem;            
        }

        void ListViewProcessCurrentObjectControllerOnCustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs customProcessListViewSelectedItemEventArgs) {
            customProcessListViewSelectedItemEventArgs.Handled = true;
        }

        void OnCustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs customizeShowViewParametersEventArgs) {
            var focusedView = (GridView) ((GridListEditor) View.Editor).Grid.FocusedView;
            ObjectSpace objectSpace = Application.GetObjectSpaceToShowViewFrom(Frame);
            object row = objectSpace.GetObject(focusedView.GetRow(focusedView.FocusedRowHandle));
            DetailView createdView = Application.CreateDetailView(objectSpace, row);
            var showViewParameters = customizeShowViewParametersEventArgs.ShowViewParameters;
            showViewParameters.CreatedView = createdView;
            showViewParameters.TargetWindow=TargetWindow.NewWindow;
        }


        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            var listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
            listViewProcessCurrentObjectController.CustomizeShowViewParameters -= OnCustomizeShowViewParameters;
            listViewProcessCurrentObjectController.CustomProcessSelectedItem -= ListViewProcessCurrentObjectControllerOnCustomProcessSelectedItem;            
        }
    }
}