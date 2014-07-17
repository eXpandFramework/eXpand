using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace ModelArtifactStateTester.Module.Controllers {
    public class TestController:ViewController {
        public TestController(){
            var simpleAction = new SimpleAction(this,"TestAction",PredefinedCategory.ObjectsCreation);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            
        }
    }
}
