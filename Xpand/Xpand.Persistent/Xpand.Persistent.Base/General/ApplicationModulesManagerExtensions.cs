using System.Linq;
using DevExpress.ExpressApp;

namespace Xpand.Persistent.Base.General{
    public static class ApplicationModulesManagerExtensions {
        public static void AddModule(this ApplicationModulesManager applicationModulesManager, XafApplication application,params ModuleBase[] moduleBases) {
            foreach (var moduleBase in moduleBases){
                moduleBase.Setup(application);
                var controllerTypes = moduleBase.GetControllerTypes().ToArray();
                applicationModulesManager.ControllersManager.RegisterControllerTypes(controllerTypes);
                applicationModulesManager.AddModule(moduleBase);
            }
        }
    }
}