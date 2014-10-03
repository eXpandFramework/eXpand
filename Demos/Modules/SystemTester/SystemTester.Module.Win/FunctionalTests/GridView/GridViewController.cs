using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace SystemTester.Module.Win.FunctionalTests.GridView {
    public class GridViewController:ViewController<ListView>{
        private const string UnboundColumn = "UnboundColumn";
        public GridViewController(){
            TargetObjectType = typeof (GridViewObject);
            var singleChoiceAction = new SingleChoiceAction(this,"GridListEditor",PredefinedCategory.ObjectsCreation);
            singleChoiceAction.Items.Add(new ChoiceActionItem(UnboundColumn, null));
            singleChoiceAction.ItemType=SingleChoiceActionItemType.ItemIsOperation;
            singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            if (e.SelectedChoiceActionItem.Id == UnboundColumn){
                var gridView = ((DevExpress.ExpressApp.Win.Editors.GridListEditor)View.Editor).GridView;
                var gridColumn = gridView.Columns.AddField("CodeUnboundColumn");
                gridColumn.Visible = true;
            }
        }

    }
}
