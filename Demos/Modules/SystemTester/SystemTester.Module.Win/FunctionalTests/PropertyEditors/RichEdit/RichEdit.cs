using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace SystemTester.Module.Win.FunctionalTests.PropertyEditors.RichEdit {
    public class RichEditController:ViewController {
        public RichEditController(){
            var simpleAction = new SimpleAction(this,"RichEdit",PredefinedCategory.ObjectsCreation) {ConfirmationMessage = "default toolbar ok"};
            simpleAction.Execute+=SimpleActionOnExecute;
            simpleAction.TargetObjectType = typeof(RichEditObject);
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            
        }
    }
}
