using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using FeatureCenter.Module.Miscellaneous.PessimisticLocking;

namespace FeatureCenter.Module.Win.Miscellaneous.PessimisticLocking {
    public class ShowNewInstanceController : ViewController<DetailView> {
        public ShowNewInstanceController() {
            TargetObjectType = typeof(PLCustomer);
            var simpleAction = new SimpleAction(this, "New instance", PredefinedCategory.Edit);
            simpleAction.Execute += SimpleActionOnExecute;
        }

        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            Process.Start(System.Windows.Forms.Application.ExecutablePath, "ApplicationMultipleInstances");
        }
    }
}
