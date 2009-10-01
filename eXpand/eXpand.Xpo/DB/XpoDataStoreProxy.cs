﻿using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace eXpand.Xpo.DB
{
    public class XpoDataStoreProxy : IDataStore
    {
        private readonly IDataLayer dataLayerCore;
        private readonly IDataStore dataStoreCore;
        #region IDataStore Members
        public XpoDataStoreProxy(string connectionString)
        {
            dataStoreCore = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.DatabaseAndSchema);
            dataLayerCore = new SimpleDataLayer(dataStoreCore);
        }
        public AutoCreateOption AutoCreateOption
        {
            get { return AutoCreateOption.DatabaseAndSchema; }
        }
        public ModificationResult ModifyData(params ModificationStatement[] dmlStatements)
        {
            var args = new DataStoreModifyDataEventArgs(dmlStatements);
            RaiseDataStoreModifyData(args);
            return dataLayerCore.ModifyData(args.ModificationStatements);
        }

        public SelectedData SelectData(params SelectStatement[] selects)
        {
            var args = new DataStoreSelectDataEventArgs(selects);
            RaiseDataStoreSelectData(args);
            return dataLayerCore.SelectData(args.SelectStatements);
        }
        public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables)
        {
            var args = new DataStoreUpdateSchemaEventArgs(dontCreateIfFirstTableNotExist, tables);
            RaiseDataStoreUpdateSchema(args);
            dataStoreCore.UpdateSchema(false, args.Tables);
            return UpdateSchemaResult.SchemaExists;
        }
        #endregion
        protected void RaiseDataStoreModifyData(DataStoreModifyDataEventArgs args)
        {
            if (DataStoreModifyData != null)
            {
                DataStoreModifyData(this, args);
            }
        }
        protected void RaiseDataStoreSelectData(DataStoreSelectDataEventArgs args)
        {
            if (DataStoreSelectData != null)
            {
                DataStoreSelectData(this, args);
            }
        }
        protected void RaiseDataStoreUpdateSchema(DataStoreUpdateSchemaEventArgs args)
        {
            if (DataStoreUpdateSchema != null)
            {
                DataStoreUpdateSchema(this, args);
            }
        }
        public event EventHandler<DataStoreModifyDataEventArgs> DataStoreModifyData;
        public event EventHandler<DataStoreSelectDataEventArgs> DataStoreSelectData;
        public event EventHandler<DataStoreUpdateSchemaEventArgs> DataStoreUpdateSchema;
    }
}