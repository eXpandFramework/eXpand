using System;
using System.IO;
using System.Web;
using DevExpress.ExpressApp;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator.Web {
    public class AssemblyPathProvider: System.AssemblyPathProvider {
        public override string GetPath(XafApplication application){
            return HttpContext.Current != null? application.GetStorageFolder(WorldCreatorModule.WCAssembliesPath)
                : (application.IsHosted() ? Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ApplicationBase): null);
        }
    }
}
