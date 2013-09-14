using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;


namespace Xpand.ExpressApp.ModelDifference.Core {
    public class ApplicationModulesManager : DevExpress.ExpressApp.ApplicationModulesManager {
        public ITypesInfo TypesInfo { get; set; }

        public ApplicationModulesManager(ControllersManager controllersManager, string assembliesPath)
            : base(controllersManager, assembliesPath) {
        }
    }

}