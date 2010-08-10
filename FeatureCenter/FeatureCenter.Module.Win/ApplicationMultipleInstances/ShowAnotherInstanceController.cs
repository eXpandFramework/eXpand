using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace FeatureCenter.Module.Win.ApplicationMultipleInstances {
    public class ShowAnotherInstanceController:ViewController<DetailView> {
        public ShowAnotherInstanceController() {
            TargetObjectType = typeof (AMIObject);
            var simpleAction = new SimpleAction(this,"Show another instance",PredefinedCategory.Tools);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            Process.Start(System.Windows.Forms.Application.ExecutablePath);
        }
    }
}