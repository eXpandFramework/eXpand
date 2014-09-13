using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace SystemTester.Module.FunctionalTests.ViewEditMode {
    public class ViewEditMode : ObjectViewController<ObjectView, ViewEditModeObject> {
        public ViewEditMode(){
            var singleChoiceAction = new SingleChoiceAction(this,"ViewEditMode",PredefinedCategory.View);
            singleChoiceAction.Items.Add(new ChoiceActionItem("ViewEditMode", null));
            singleChoiceAction.ItemType=SingleChoiceActionItemType.ItemIsOperation;
            singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            var showViewParameters = e.ShowViewParameters;
            showViewParameters.TargetWindow=TargetWindow.NewModalWindow;
            var objectSpace = Application.CreateObjectSpace();
            var viewEditModeObject = objectSpace.GetObject(e.SelectedObjects[0]);
            showViewParameters.CreatedView = Application.CreateDetailView(objectSpace, ((ListView)View).Model.DetailView, true, viewEditModeObject);
        }
    }
}
