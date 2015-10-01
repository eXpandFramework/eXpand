using System;
using System.Linq;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Dashboard {
    public static class Extensions {
        public static object CreateDashboardDataSource(this XafApplication application, Type objectType) {
            var space = application.CreateObjectSpace(objectType);
            var proxyCollection = new ProxyCollection(space, space.TypesInfo.FindTypeInfo(objectType), space.GetObjects(objectType));
            proxyCollection.DisplayableMembers = string.Join(";", proxyCollection.DisplayableMembers.Split(';').Where(s => !s.EndsWith("!")));
            return proxyCollection;
        }
    }
}
