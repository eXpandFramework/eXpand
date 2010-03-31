using System;
using System.Data;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace eXpand.Xpo.DB
{
    public class XpoDataStoreProxy : IXpoDataStoreProxy {
        private readonly IDataLayer dataLayerCore;
        private readonly ISqlDataStore dataStoreCore;

        protected XpoDataStoreProxy() {
        }
        #region IDataStore Members
        public XpoDataStoreProxy(string connectionString){
            dataStoreCore = (ISqlDataStore) XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.DatabaseAndSchema);
            dataLayerCore = new SimpleDataLayer(dataStoreCore);
        }

        public static implicit operator ConnectionProviderSql(XpoDataStoreProxy proxy){
            return proxy.dataStoreCore as ConnectionProviderSql;
        }

        public AutoCreateOption AutoCreateOption{
            get { return AutoCreateOption.DatabaseAndSchema; }
        }
        public virtual ModificationResult ModifyData(params ModificationStatement[] dmlStatements){
            var args = new DataStoreModifyDataEventArgs(dmlStatements);
            OnDataStoreModifyData(args);
            return args.ModificationResult?? dataLayerCore.ModifyData(args.ModificationStatements);
        }

        public virtual SelectedData SelectData(params SelectStatement[] selects){
            var args = new DataStoreSelectDataEventArgs(selects);
            OnDataStoreSelectData(args);
            return args.SelectedData ?? dataLayerCore.SelectData(args.SelectStatements);
        }
        public virtual UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables){
            var args = new DataStoreUpdateSchemaEventArgs(dontCreateIfFirstTableNotExist, tables);
            OnDataStoreUpdateSchema(args);
            return dataStoreCore.UpdateSchema(false, args.Tables);
        }
        #endregion
        protected void OnDataStoreModifyData(DataStoreModifyDataEventArgs args){
            if (DataStoreModifyData != null) {
                DataStoreModifyData(this, args);
            }
        }
        protected void OnDataStoreSelectData(DataStoreSelectDataEventArgs args){
            if (DataStoreSelectData != null) {
                DataStoreSelectData(this, args);
            }
        }
        protected void OnDataStoreUpdateSchema(DataStoreUpdateSchemaEventArgs args){
            if (DataStoreUpdateSchema != null) {
                DataStoreUpdateSchema(this, args);
            }
        }
        public event EventHandler<DataStoreModifyDataEventArgs> DataStoreModifyData;
        public event EventHandler<DataStoreSelectDataEventArgs> DataStoreSelectData;
        public event EventHandler<DataStoreUpdateSchemaEventArgs> DataStoreUpdateSchema;
        public IDbConnection Connection {
            get { return dataStoreCore.Connection; }
        }

        public IDbCommand CreateCommand() {
            return dataStoreCore.CreateCommand();
        }
    }
}