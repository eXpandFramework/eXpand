using DevExpress.ExpressApp.Xpo;
using Xpand.Xpo.DB;

namespace Xpand.ExpressApp {
    public interface IXpoDataStoreProxy : IXpoDataStoreProvider {
        DataStoreProxy Proxy { get; }
    }
}