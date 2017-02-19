using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;

namespace Xpand.Persistent.Base.ModelDifference {
    class XpandApplicationModulesManager : ApplicationModulesManager {
        public ITypesInfo TypesInfo { get; set; }

        public XpandApplicationModulesManager(ControllersManager controllersManager, string assembliesPath)
            : base(controllersManager, assembliesPath) {
        }

        public void AddAdditionalModules(XafApplication application){
            foreach (var m in Modules.OfType<IAdditionalModuleProvider>().ToArray()){
                ((ModuleBase) m).Application = application;
                m.AddAdditionalModules(this);
            }
        }
    }

    public interface IAdditionalModuleProvider{

        void AddAdditionalModules(ApplicationModulesManager applicationModulesManager);
    }
}