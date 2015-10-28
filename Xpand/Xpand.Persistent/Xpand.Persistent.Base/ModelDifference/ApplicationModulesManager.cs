using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;

namespace Xpand.Persistent.Base.ModelDifference {
    class XpandApplicationModulesManager : ApplicationModulesManager {
        public ITypesInfo TypesInfo { get; set; }

        public XpandApplicationModulesManager(ControllersManager controllersManager, string assembliesPath)
            : base(controllersManager, assembliesPath) {
        }
    }

}