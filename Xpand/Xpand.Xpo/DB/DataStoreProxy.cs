using System;
using System.Collections.Generic;
using System.Data;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;

namespace Xpand.Xpo.DB {
    public class DataStoreProxy : IDataStoreProxy {
        private BaseDataLayer _dataLayerCore;
        private readonly IDataStore _dataStoreCore;
        protected readonly List<ISchemaUpdater> SchemaUpdaters = new List<ISchemaUpdater> { new SchemaColumnSizeUpdater() };
        protected DataStoreProxy() {
        }
        #region IDataStore Members

        public DataStoreProxy(IDataStore dataStore) {
            _dataStoreCore = dataStore;
        }

        public DataStoreProxy(string connectionString,AutoCreateOption autoCreateOption=AutoCreateOption.None):this(XpoDefault.GetConnectionProvider(connectionString, autoCreateOption)){
            
        }

        public static implicit operator ConnectionProviderSql(DataStoreProxy proxy) {
            return proxy._dataStoreCore as ConnectionProviderSql;
        }

        public IDataStore DataStore {
            get { return _dataStoreCore; }
        }

        public AutoCreateOption AutoCreateOption {
            get { return _dataStoreCore.AutoCreateOption; }
        }
        public virtual ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
            var args = new DataStoreModifyDataEventArgs(dmlStatements);
            OnDataStoreModifyData(args);
            return args.ModificationResult ?? _dataStoreCore.ModifyData(args.ModificationStatements);
        }

        public virtual SelectedData SelectData(params SelectStatement[] selects) {
            var args = new DataStoreSelectDataEventArgs(selects);
            OnDataStoreSelectData(args);
            return args.SelectedData ?? _dataStoreCore.SelectData(args.SelectStatements);
        }

        public void RegisterSchenameUpdater(ISchemaUpdater schemaUpdater) {
            SchemaUpdaters.Add(schemaUpdater);
        }

        public virtual UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
            foreach (var schemaUpdater in SchemaUpdaters) {
                schemaUpdater.Update(this, new DataStoreUpdateSchemaEventArgs(dontCreateIfFirstTableNotExist, tables));
            }
            var args = new DataStoreUpdateSchemaEventArgs(dontCreateIfFirstTableNotExist, tables);
            OnDataStoreUpdateSchema(args);
            return _dataStoreCore.UpdateSchema(false, args.Tables);
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
                var sqlDataStore = (_dataStoreCore as ISqlDataStore);
                return sqlDataStore != null ? sqlDataStore.Connection : null;
            }
        }

        public IDbCommand CreateCommand() {
            return ((ISqlDataStore)_dataStoreCore).CreateCommand();
        }

        object ICommandChannel.Do(string command, object args) {
            if (_dataLayerCore == null)
                _dataLayerCore = new SimpleDataLayer(_dataStoreCore);
            return ((ICommandChannel)_dataLayerCore).Do(command, args);
        }
    }
}