using DevExpress.ExpressApp;
using eXpand.Xpo.DB;

namespace eXpand.ExpressApp {
    public interface IXpoDataStoreProxy : IXpoDataStoreProvider{
        SqlDataStoreProxy Proxy { get; }
    }
}