using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Security.Controllers {
    public abstract class RememberMeController : WindowController {
        protected RememberMeController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated() {
            base.OnActivated();
            Application.LoggingOff += ApplicationOnLoggingOff;
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Application.LoggingOff -= ApplicationOnLoggingOff;
        }

        protected abstract void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs);
    }
}
