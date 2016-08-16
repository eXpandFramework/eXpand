using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace SystemTester.Module.Web.FunctionalTests.MasterDetail {
    public class MasterDetail:ViewController<ListView>{
        public MasterDetail(){
            TargetObjectType = typeof (Master);
            var singleChoiceAction = new SimpleAction(this,"CreateMaster",PredefinedCategory.View) {Caption = "Create"};
            singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }

        private void SingleChoiceActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var master = ObjectSpace.CreateObject<Master>();
            master.Name = "Master";
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

    }
}
