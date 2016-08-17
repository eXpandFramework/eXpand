using DevExpress.ExpressApp;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator.Win {
    public class AssemblyPathProvider: System.AssemblyPathProvider {
        public override string GetPath(XafApplication application){
            return application.GetStorageFolder(WorldCreatorModule.WCAssembliesPath);
        }
    }
}
