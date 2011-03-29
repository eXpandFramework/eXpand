using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;

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
            var lastLayer = modelApplicationBase.LastLayer;
            while (lastLayer.Id != "Unchanged Master Part") {
                modelApplicationBase.RemoveLayer(lastLayer);
                lastLayer = modelApplicationBase.LastLayer;
            }
            var afterSetupLayer = modelApplicationBase.CreatorInstance.CreateModelApplication();
            afterSetupLayer.Id = "After Setup";
            modelApplicationBase.AddLayer(afterSetupLayer);
        }

    }
}
