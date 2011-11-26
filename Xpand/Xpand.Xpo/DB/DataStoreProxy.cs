using System;
using System.Collections.Generic;
using System.Data;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;

namespace Xpand.Xpo.DB {
    public class DataStoreProxy : IDataStoreProxy {
        private readonly BaseDataLayer dataLayerCore;
        private readonly IDataStore dataStoreCore;
        protected readonly List<ISchemaUpdater> schemaUpdaters = new List<ISchemaUpdater> { new SchemaColumnSizeUpdater() };
        protected DataStoreProxy() {
        }
        #region IDataStore Members
        public DataStoreProxy(string connectionString) {
            dataStoreCore = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.DatabaseAndSchema);
            dataLayerCore = new SimpleDataLayer(dataStoreCore);
        }

        public DataStoreProxy(IDataStore dataStore) {
            dataStoreCore = dataStore;
            dataLayerCore = new SimpleDataLayer(dataStoreCore);
        }

        public static implicit operator ConnectionProviderSql(DataStoreProxy proxy) {
            return proxy.dataStoreCore as ConnectionProviderSql;
        }

        public IDataStore DataStore {
            get { return dataStoreCore; }
        }

        public AutoCreateOption AutoCreateOption {
            get { return AutoCreateOption.DatabaseAndSchema; }
        }
        public virtual ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
            var args = new DataStoreModifyDataEventArgs(dmlStatements);
            OnDataStoreModifyData(args);
            return args.ModificationResult ?? dataLayerCore.ModifyData(args.ModificationStatements);
        }

        public virtual SelectedData SelectData(params SelectStatement[] selects) {
            var args = new DataStoreSelectDataEventArgs(selects);
            OnDataStoreSelectData(args);
            return args.SelectedData ?? dataLayerCore.SelectData(args.SelectStatements);
        }

        public void RegisterSchenameUpdater(ISchemaUpdater schemaUpdater) {
            schemaUpdaters.Add(schemaUpdater);
        }

        public virtual UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
            foreach (var schemaUpdater in schemaUpdaters) {
                schemaUpdater.Update(this, new DataStoreUpdateSchemaEventArgs(dontCreateIfFirstTableNotExist, tables));
            }
            var args = new DataStoreUpdateSchemaEventArgs(dontCreateIfFirstTableNotExist, tables);
            OnDataStoreUpdateSchema(args);
            return dataStoreCore.UpdateSchema(false, args.Tables);
        }
        #endregion
        protected void OnDataStoreModifyData(DataStoreModifyDataEventArgs args) {
            if (DataStoreModifyData != null) {
                DataStoreModifyData(this, args);
            }
        }
        protected void OnDataStoreSelectData(DataStoreSelectDataEventArgs args) {
            if (DataStoreSelectData != null) {
                DataStoreSelectData(this, args);
            }
        }
        protected void OnDataStoreUpdateSchema(DataStoreUpdateSchemaEventArgs args) {
            if (DataStoreUpdateSchema != null) {
                DataStoreUpdateSchema(this, args);
            }
        }
        public event EventHandler<DataStoreModifyDataEventArgs> DataStoreModifyData;
        public event EventHandler<DataStoreSelectDataEventArgs> DataStoreSelectData;
        public event EventHandler<DataStoreUpdateSchemaEventArgs> DataStoreUpdateSchema;

        public IDbConnection Connection {
            get {
                var sqlDataStore = (dataStoreCore as ISqlDataStore);
                return sqlDataStore != null ? sqlDataStore.Connection : null;
            }
        }

        public IDbCommand CreateCommand() {
            return ((ISqlDataStore)dataStoreCore).CreateCommand();
        }

        object ICommandChannel.Do(string command, object args) {
            return ((ICommandChannel)dataLayerCore).Do(command, args);
        }
    }
}