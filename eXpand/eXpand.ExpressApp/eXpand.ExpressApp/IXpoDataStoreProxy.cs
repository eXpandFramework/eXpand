using DevExpress.ExpressApp;
using eXpand.Xpo.DB;

namespace eXpand.ExpressApp {
    public interface IXpoDataStoreProxy : IXpoDataStoreProvider{
        XpoDataStoreProxy Proxy { get; }
    }
}