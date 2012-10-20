using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.MessageBox;

namespace FeatureCenter.Module.MessageBox {
    public class ShowMessageBoxController : ViewController {
        public ShowMessageBoxController() {
            TargetObjectType = typeof(ShowMessageBoxObject);
            var showOkAndCancel = new SimpleAction(this, "ShowOkAndCancel", PredefinedCategory.OpenObject);
            showOkAndCancel.Execute += ShowOkAndCancelOnExecute;
        }

        void ShowOkAndCancelOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            new GenericMessageBox(simpleActionExecuteEventArgs.ShowViewParameters, Application,
                                  "Press Ok or press cancel", Accept, Cancel);
        }

        void Cancel(object sender, EventArgs eventArgs) {

        }

        void Accept(object sender, ShowViewParameters showViewParameters) {
            new GenericMessageBox(showViewParameters, Application, "Ok button pressed");
        }
    }
}
