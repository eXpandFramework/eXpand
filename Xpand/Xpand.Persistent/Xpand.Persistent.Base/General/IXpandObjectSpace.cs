using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Xpo.DB;

namespace Xpand.Persistent.Base.General {
    public interface IXpandObjectSpace:IObjectSpace {
        object FindObject(Type objectType, DevExpress.Data.Filtering.CriteriaOperator criteria, bool inTransaction,
                          bool selectDeleted);
    }
    public interface IXpandObjectSpaceProvider : IObjectSpaceProvider {
        IDataLayer WorkingDataLayer { get; }
        IXpoDataStoreProxy DataStoreProvider { get; set; }
        event EventHandler<CreatingWorkingDataLayerArgs> CreatingWorkingDataLayer;
    }
    public interface IXpoDataStoreProxy : IXpoDataStoreProvider {
        DataStoreProxy Proxy { get; }
    }
    public class CreatingWorkingDataLayerArgs : EventArgs {
        readonly IDataStore _workingDataStore;

        public CreatingWorkingDataLayerArgs(IDataStore workingDataStore) {
            _workingDataStore = workingDataStore;
        }

        public IDataStore WorkingDataStore {
            get { return _workingDataStore; }
        }

        public IDataLayer DataLayer { get; set; }
    }

}