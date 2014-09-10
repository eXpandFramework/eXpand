using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace SystemTester.Module.FunctionalTests.RunTimeMembers {
    public class RunTimeMembers : ObjectViewController<ObjectView, RunTimeMembersObject> {
        public RunTimeMembers(){
            var simpleAction = new SimpleAction(this,"RunTimeMembersObject",PredefinedCategory.View);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            var objectSpace = Application.CreateObjectSpace();
            e.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace,objectSpace.CreateObject<RunTimeMembersObjectConfig>());
            e.ShowViewParameters.TargetWindow=TargetWindow.NewModalWindow;
        }
    }
}
