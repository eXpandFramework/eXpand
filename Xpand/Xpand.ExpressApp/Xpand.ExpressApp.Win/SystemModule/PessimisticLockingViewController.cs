using DevExpress.ExpressApp.Actions;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class PessimisticLockingViewController : Persistent.Base.General.Controllers.PessimisticLockingViewController {
        protected override void SubscribeToEvents() {
            base.SubscribeToEvents();
            Frame.GetController<SwitchToEditModeController>().EditAction.Execute+=EditActionOnExecute;
        }
        
        void EditActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            UpdateViewAllowEditState();
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<SwitchToEditModeController>().EditAction.Execute -= EditActionOnExecute;
        }
    }
}
