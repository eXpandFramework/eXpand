using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General {
    public class ApplicationHelper {
        private const string ValueManagerKey = "ApplicationHelper";
        private static volatile IValueManager<ApplicationHelper> _instanceValueManager;
        private static readonly object SyncRoot = new object();
        private XafApplication _application;
        private ApplicationHelper() {
        }
        public static ApplicationHelper Instance {
            get {
                if (_instanceValueManager == null) {
                    lock (SyncRoot) {
                        if (_instanceValueManager == null) {
                            _instanceValueManager = ValueManager.GetValueManager<ApplicationHelper>(ValueManagerKey);
                        }
                    }
                }
                if (_instanceValueManager.Value == null) {
                    lock (SyncRoot) {
                        if (_instanceValueManager.Value == null) {
                            _instanceValueManager.Value = new ApplicationHelper();
                        }
                    }
                }
                return _instanceValueManager.Value;
            }
        }
        public XafApplication Application => _application;

        public void Initialize(XafApplication application) {
            _application = application;
        }
    }
}
