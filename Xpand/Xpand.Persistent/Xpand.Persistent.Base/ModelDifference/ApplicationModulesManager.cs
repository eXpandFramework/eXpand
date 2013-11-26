using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;

namespace Xpand.Persistent.Base.ModelDifference {
    class XpandApplicationModulesManager : DevExpress.ExpressApp.ApplicationModulesManager {
        public ITypesInfo TypesInfo { get; set; }

        public XpandApplicationModulesManager(ControllersManager controllersManager, string assembliesPath)
            : base(controllersManager, assembliesPath) {
        }
    }

}