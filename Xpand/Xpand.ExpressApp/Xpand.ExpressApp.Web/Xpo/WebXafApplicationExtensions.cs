using System.Web;
using DevExpress.ExpressApp.Xpo;

namespace Xpand.ExpressApp.Web.Xpo {
    public static class WebXafApplicationExtensions{

        public static IXpoDataStoreProvider CachedInstance(this IXpoDataStoreProvider dataStoreProvider) {
            if (dataStoreProvider.ConnectionString == InMemoryDataStoreProvider.ConnectionString)
                dataStoreProvider=new MemoryDataStoreProvider();
            string key = dataStoreProvider.GetType().Name;
            if (HttpContext.Current.Application[key] != null)
                return (IXpoDataStoreProvider)HttpContext.Current.Application[key];
            HttpContext.Current.Application[key] = dataStoreProvider;
            return dataStoreProvider;
        }
    }
}