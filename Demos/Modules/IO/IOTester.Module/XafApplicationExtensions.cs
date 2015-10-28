using System.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General;

namespace IOTester.Module {
    public static class XafApplicationExtensions {
        public static IObjectSpaceProvider GetObjectSpaceProvider(this XafApplication application,string connectionstring) {
            if (!application.GetEasyTestParameter("NorthWind")){
                return new XPObjectSpaceProvider(connectionstring, (IDbConnection) null, application.IsHosted());
            }
            var args = new CreateCustomObjectSpaceProviderEventArgs(connectionstring);
            application.CreateCustomObjectSpaceprovider(args, null);
            return args.ObjectSpaceProvider;
        }
    }
}
