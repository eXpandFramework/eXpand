using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;

namespace Xpand.ExpressApp {
    public interface IXpandObjectSpaceProvider : IObjectSpaceProvider {
        IDataLayer WorkingDataLayer { get; }
        IXpoDataStoreProxy DataStoreProvider { get; set; }
        event EventHandler<CreatingWorkingDataLayerArgs> CreatingWorkingDataLayer;
    }
}