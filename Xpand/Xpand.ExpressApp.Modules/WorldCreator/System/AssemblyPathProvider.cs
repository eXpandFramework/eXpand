using System.Linq;
using DevExpress.ExpressApp;
using Fasterflect;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator.System {
    public class AssemblyPathProvider{
        static AssemblyPathProvider(){
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(typeof(AssemblyPathProvider)).Descendants.FirstOrDefault();
            Instance = typeInfo != null? (AssemblyPathProvider) typeInfo.Type.CreateInstance(): new AssemblyPathProvider();
        }

        public static AssemblyPathProvider Instance { get; }

        public virtual string GetPath(XafApplication application){
            return application.GetStorageFolder(WorldCreatorModule.WCAssembliesPath);
        }
    }
}
