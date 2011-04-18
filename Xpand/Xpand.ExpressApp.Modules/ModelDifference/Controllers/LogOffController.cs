using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.ModelDifference.Controllers {
    public class LogOffController : WindowController {
        public LogOffController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated() {
            base.OnActivated();
            Application.LoggingOff +=ApplicationOnLoggingOff;
        }

        void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs) {
            if (!loggingOffEventArgs.CanCancel)
                return;
            var modelApplicationBase = ((ModelApplicationBase)((XafApplication)sender).Model);
            modelApplicationBase.ReInitLayers();
            
        }

    }
}
