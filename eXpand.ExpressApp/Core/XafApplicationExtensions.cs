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

        public static void DatabaseVersionMismatchEvent(this XafApplication xafApplication, object sender, DatabaseVersionMismatchEventArgs e)
        {

            try
            {
                e.Updater.Update();
            }
            catch (CompatibilityException e1)
            {
                if (e1.Error is CompatibilityCheckVersionsError)
                    return;
                throw;
            }
            e.Handled = true;

        }

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args)
        {
            args.ObjectSpaceProvider = new ObjectSpaceProvider(new DataStoreProvider(args.ConnectionString));
        }
    }
}