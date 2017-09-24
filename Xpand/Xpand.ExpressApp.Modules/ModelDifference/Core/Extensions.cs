using DevExpress.ExpressApp;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.Core {
    public static class Extensions {
        public static bool ApplicationDeviceModelsEnabled(this XafApplication application){
            return application.GetPlatform() == Platform.Web&& ((IModelOptionsModelDifference) application.Model.Options).ApplicationMobileModels;
        }
        public static bool UserDeviceModelsEnabled(this XafApplication application){
            return application.GetPlatform() == Platform.Web&& ((IModelOptionsModelDifference)application.Model.Options).UserMobileModels;
        }

    }
}
