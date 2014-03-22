using System;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Dashboard {
    public static class Extensions {
        public static object CreateDashboardDataSource(this IObjectSpace objectSpace,Type objectType) {
            return objectSpace.CreateServerCollection(objectType, null);
        }
    }
}
