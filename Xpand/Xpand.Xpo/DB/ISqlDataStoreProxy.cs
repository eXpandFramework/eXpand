using System;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;

namespace Xpand.Xpo.DB {
    public interface ISqlDataStoreProxy : ISqlDataStore,ICommandChannel {
        event EventHandler<DataStoreModifyDataEventArgs> DataStoreModifyData;
        event EventHandler<DataStoreSelectDataEventArgs> DataStoreSelectData;
        event EventHandler<DataStoreUpdateSchemaEventArgs> DataStoreUpdateSchema;
        ISqlDataStore DataStore { get;  }
    }

    public interface ISchemaUpdater {
        void Update(ConnectionProviderSql proxy, DataStoreUpdateSchemaEventArgs dataStoreUpdateSchemaEventArgs);
    }
}