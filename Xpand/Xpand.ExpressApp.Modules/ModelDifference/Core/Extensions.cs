using System.IO;
using DevExpress.ExpressApp;
using Fasterflect;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.Core {
    public static class Extensions {
        public static string GetModelFilePath(this XafApplication application,string filePath=null) {
            var path = Path.Combine(Path.GetTempPath(),$"{application.GetType().Name}ModelAssembly.dll");
            return application.GetPlatform() == Platform.Win ? filePath ?? path : application.GetFieldValue("sharedModelManager") == null ? path : null;
        }

        public static bool ApplicationDeviceModelsEnabled(this XafApplication application){
            return application.GetPlatform() == Platform.Web&& ((IModelOptionsModelDifference) application.Model.Options).ApplicationMobileModels;
        }
        public static bool UserDeviceModelsEnabled(this XafApplication application){
            return application.GetPlatform() == Platform.Web&& ((IModelOptionsModelDifference)application.Model.Options).UserMobileModels;
        }

    }
}
