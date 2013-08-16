using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic {
    public class LogicInstallerManager {
        readonly List<ILogicInstaller> _logicInstallers = new List<ILogicInstaller>();
        static IValueManager<LogicInstallerManager> _instanceManager;

        LogicInstallerManager() {
        }

        public static LogicInstallerManager Instance {
            get {
                if (_instanceManager == null) {
                    _instanceManager = ValueManager.GetValueManager<LogicInstallerManager>("LogicInstallerManager");
                }
                return _instanceManager.Value ?? (_instanceManager.Value = new LogicInstallerManager());
            }
        }

        public ReadOnlyCollection<ILogicInstaller> LogicInstallers {
            get { return _logicInstallers.AsReadOnly(); }
        }

        public static void RegisterInstaller(ILogicInstaller logicInstaller) {
            Instance._logicInstallers.Add(logicInstaller);
        }

        public static void RegisterInstallers(IEnumerable<ILogicInstaller> logicInstallers) {
            Instance._logicInstallers.AddRange(logicInstallers);
        }
    }
}