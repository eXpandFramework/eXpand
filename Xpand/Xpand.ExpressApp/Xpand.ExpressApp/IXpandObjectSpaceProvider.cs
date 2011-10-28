using System;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp {
    public interface IXpandObjectSpaceProvider : IObjectSpaceProvider {
        IXpoDataStoreProxy DataStoreProvider { get; set; }
        event EventHandler<CreatingWorkingDataLayerArgs> CreatingWorkingDataLayer;
    }
}