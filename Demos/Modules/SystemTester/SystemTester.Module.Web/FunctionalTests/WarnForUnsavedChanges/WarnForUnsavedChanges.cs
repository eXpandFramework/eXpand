using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using Xpand.Persistent.Base.General.Controllers;

namespace SystemTester.Module.Web.FunctionalTests.WarnForUnsavedChanges{
    public class WarnForUnsavedChanges:ObjectViewController<ObjectView,WarnForUnsavedChangesObject>{
        protected override void OnActivated(){
            base.OnActivated();
            Frame.GetController<EasyTestController>().ParametrizedAction.Execute+=ParametrizedActionOnExecute;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            Frame.GetController<EasyTestController>().ParametrizedAction.Execute -= ParametrizedActionOnExecute;
        }

        private void ParametrizedActionOnExecute(object sender, ParametrizedActionExecuteEventArgs parametrizedActionExecuteEventArgs){
            ObjectSpace.CommitChanges();
        }
    }
}