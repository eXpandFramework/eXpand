using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Core;

namespace WizardUITester.Module.Win.FunctionalTests{
    public class ActionViewController:ViewController{
        public ActionViewController(){
            var simpleAction = new SimpleAction(this, "Page1Action", "Page1");
            simpleAction.Execute+=SIMPLEActionOnExecute;
            simpleAction = new SimpleAction(this, "Page2Action", "Page2");
            simpleAction.Execute+=SIMPLEActionOnExecute;
        }

        private void SIMPLEActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            var caption = ((SimpleAction) sender).Caption;
            Messaging.GetMessaging(Application).Show(caption, caption);
        }
    }
}