using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace ModelArtifactStateTester.Module.FunctionalTests.Actions {
    public class Actions:ViewController {
        public Actions(){
            var simpleAction = new SimpleAction(this,"Test1Action",PredefinedCategory.View);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            
        }
    }
}
