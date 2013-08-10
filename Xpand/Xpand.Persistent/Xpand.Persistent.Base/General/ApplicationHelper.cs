using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General {
    public class ApplicationHelper {
        private const string ValueManagerKey = "ApplicationHelper";
        private static volatile IValueManager<ApplicationHelper> instanceValueManager;
        private static readonly object syncRoot = new object();
        private XafApplication _application;
        private ApplicationHelper() {
        }
        public static ApplicationHelper Instance {
            get {
                if (instanceValueManager == null) {
                    lock (syncRoot) {
                        if (instanceValueManager == null) {
                            instanceValueManager = ValueManager.GetValueManager<ApplicationHelper>(ValueManagerKey);
                        }
                    }
                }
                if (instanceValueManager.Value == null) {
                    lock (syncRoot) {
                        if (instanceValueManager.Value == null) {
                            instanceValueManager.Value = new ApplicationHelper();
                        }
                    }
                }
                return instanceValueManager.Value;
            }
        }
        public XafApplication Application { get { return _application; } }

        public void Initialize(XafApplication application) {
            if (_application==null)
                _application = application;
        }
    }
}
