using System;
using DevExpress.Xpo.DB;

namespace eXpand.Xpo.DB {
    public interface IXpoDataStoreProxy : ISqlDataStore {
        event EventHandler<DataStoreModifyDataEventArgs> DataStoreModifyData;
        event EventHandler<DataStoreSelectDataEventArgs> DataStoreSelectData;
        event EventHandler<DataStoreUpdateSchemaEventArgs> DataStoreUpdateSchema;
    }
}