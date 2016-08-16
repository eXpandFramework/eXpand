using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.BaseImpl.Security;

namespace SystemTester.Module.FunctionalTests.PessimisticLocking {
    public class PessimisticLocking:ObjectViewController<ListView,PessimisticLockingObject>{
        public PessimisticLocking(){
            var simpleAction = new SimpleAction(this,"MarkModified",PredefinedCategory.View){Caption = "Modify"};
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var selectedObject = View.SelectedObjects[0];
            var member = View.ObjectTypeInfo.FindMember(PessimisticLockingViewController.LockedUser);
            member.SetValue(selectedObject,ObjectSpace.QueryObject<XpandPermissionPolicyUser>(user => user.UserName=="User"));
            ObjectSpace.CommitChanges();
        }
    }
}
