using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace ModelArtifactStateTester.Module.Win.FunctionalTests.Actions.Win {
    public class Actions:Module.FunctionalTests.Actions.Actions {
        public Actions(){
            var simpleAction = new SimpleAction(this, "Test2Action", PredefinedCategory.View);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            
        }
    }
}
