using System;
using DevExpress.Xpo.DB;

namespace Xpand.Xpo.DB {
    public interface ISqlDataStoreProxy : ISqlDataStore {
        event EventHandler<DataStoreModifyDataEventArgs> DataStoreModifyData;
        event EventHandler<DataStoreSelectDataEventArgs> DataStoreSelectData;
        event EventHandler<DataStoreUpdateSchemaEventArgs> DataStoreUpdateSchema;
    }
}