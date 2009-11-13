using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using eXpand.Xpo.DB;

namespace eXpand.ExpressApp.Core
{
    public static class XafApplicationExtensions
    {
        public static DetailView CreateDetailView(this XafApplication xafApplication, Type objectType)
        {
            ObjectSpace objectSpace = xafApplication.CreateObjectSpace();
            return xafApplication.CreateDetailView(objectSpace, Activator.CreateInstance(objectType, new object[] { objectSpace.Session }));
        }
        public static string GetConnectionString(this XafApplication xafApplication) {
            IDataStore connectionProvider = ((BaseDataLayer)xafApplication.ObjectSpaceProvider.CreateObjectSpace().Session.DataLayer).ConnectionProvider;
            if (connectionProvider is XpoDataStoreProxy)
                return ((ConnectionProviderSql)((XpoDataStoreProxy)connectionProvider)).ConnectionString;
            return ((ConnectionProviderSql)(connectionProvider)).ConnectionString;
        }

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args)
        {
            args.ObjectSpaceProvider = new ObjectSpaceProvider(new DataStoreProvider(args.ConnectionString));
        }
    }
}