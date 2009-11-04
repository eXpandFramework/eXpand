using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace eXpand.ExpressApp.Core
{
    public static class XafApplicationExtensions
    {
        public static DetailView CreateDetailView(this XafApplication xafApplication, Type objectType)
        {
            ObjectSpace objectSpace = xafApplication.CreateObjectSpace();
            return xafApplication.CreateDetailView(objectSpace, Activator.CreateInstance(objectType, new object[] { objectSpace.Session }));
        }

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args)
        {
            args.ObjectSpaceProvider = new ObjectSpaceProvider(new DataStoreProvider(args.ConnectionString));
        }
    }
}