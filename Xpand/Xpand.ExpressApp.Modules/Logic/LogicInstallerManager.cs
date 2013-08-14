using System.Collections.Generic;
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

        public List<ILogicInstaller> LogicInstallers {
            get { return _logicInstallers; }
        }

        public void RegisterInstaller(ILogicInstaller logicInstaller) {
            _logicInstallers.Add(logicInstaller);
        }

        public void RegisterInstallers(IEnumerable<ILogicInstaller> logicInstallers) {
            _logicInstallers.AddRange(logicInstallers);
        }
    }
}