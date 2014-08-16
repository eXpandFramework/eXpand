using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace XpandSystemTester.Module.Win.FunctionalTests.GridView {
    public class GridViewController:ViewController<ListView>{
        private const string UnboundColumn = "UnboundColumn";
        public GridViewController(){
            TargetObjectType = typeof (GridViewObject);
            var singleChoiceAction = new SingleChoiceAction(this,"GridListEditor",PredefinedCategory.ObjectsCreation);
            singleChoiceAction.Items.Add(new ChoiceActionItem(UnboundColumn, null));
            singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs){
            if (singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Id == UnboundColumn){
                var gridView = ((DevExpress.ExpressApp.Win.Editors.GridListEditor)View.Editor).GridView;
                gridView.Columns.AddField("Test");
            }
        }

    }
}
