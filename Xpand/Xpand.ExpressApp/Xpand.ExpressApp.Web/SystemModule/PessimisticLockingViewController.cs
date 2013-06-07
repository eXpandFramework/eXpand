
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class PessimisticLockingViewController : ExpressApp.SystemModule.PessimisticLockingViewController {
        protected override void SubscribeToEvents() {
            base.SubscribeToEvents();
            Frame.GetController<WebModificationsController>().EditAction.Execute += EditActionOnExecute;
        }

        void EditActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            UpdateViewAllowEditState();
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<WebModificationsController>().EditAction.Execute -= EditActionOnExecute;
        }
    }
}
