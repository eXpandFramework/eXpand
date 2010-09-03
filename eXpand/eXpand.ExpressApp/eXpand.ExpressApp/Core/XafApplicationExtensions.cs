using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Core
{
    public static class XafApplicationExtensions
    {

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args)
        {
            args.ObjectSpaceProvider = new XpandObjectSpaceProvider(new MultiDataStoreProvider(args.ConnectionString));
        }
    }
}