using DevExpress.ExpressApp.Actions;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class PessimisticLockingViewController : Persistent.Base.General.Controllers.PessimisticLockingViewController {
        protected override void SubscribeToEvents() {
            base.SubscribeToEvents();
            Frame.GetController<SwitchToEditModeController>(controller => controller.EditAction.Execute += EditActionOnExecute);
        }
        
        void EditActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            UpdateViewAllowEditState();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<SwitchToEditModeController>(controller => controller.EditAction.Execute -= EditActionOnExecute);
        }
    }
}
