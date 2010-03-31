using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using eXpand.Xpo.DB;

namespace eXpand.ExpressApp {
    public class _MultiDataStoreProvider : DataStoreProvider
    {
        XpoMultiDataStoreProxy _xpoMultiDataStoreproxy;

        public _MultiDataStoreProvider(string connectionString) : base(connectionString) {
        }
        public override XpoDataStoreProxy Proxy
        {
            get
            {
                if (_xpoMultiDataStoreproxy == null)
                    _xpoMultiDataStoreproxy = new XpoMultiDataStoreProxy(ConnectionString);
                return base.Proxy;
            }
        }
    }

    public class XpoMultiDataStoreProxy : XpoDataStoreProxy
    {
        DataStoreManager _dataStoreManager;

        protected XpoMultiDataStoreProxy() {
        }

        public XpoMultiDataStoreProxy(string connectionString, XPDictionary xpDictionary): base(connectionString)
        {
            _dataStoreManager = new DataStoreManager(connectionString);
            FillDictionaries(xpDictionary);
        }
        public DataStoreManager DataStoreManager
        {
            get { return _dataStoreManager; }
        }

        public override ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
            
        }

        public override SelectedData SelectData(params SelectStatement[] selects) {
            var resultSet = new List<SelectStatementResult>();
            List<SelectedData> selectedDatas = selects.Select(stm =>
                    _dataStoreManager.SimpleDataLayers[_dataStoreManager.GetKey(stm.TableName)].SelectData(stm)).ToList();
            foreach (SelectedData selectedData in selectedDatas.Where(
                selectedData => selectedData != null))
            {
                resultSet.AddRange(selectedData.ResultSet);
            }
            args.SelectData = new SelectedData(resultSet.ToArray());

        }

        public override UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
            foreach (KeyValuePair<IDataStore, List<DBTable>> dataStore in _dataStoreManager.GetDataStores(tables)){
                IDataStore store = dataStore.Key;
                List<DBTable> dbTables = dataStore.Value;

                store.UpdateSchema(false, dbTables.ToArray());
            }
            return UpdateSchemaResult.SchemaExists;
        }

        void FillDictionaries(XPDictionary xpDictionary)
        {
            foreach (XPClassInfo queryClassInfo in xpDictionary.Classes.OfType<XPClassInfo>().Where(info => !(info is IntermediateClassInfo)))
            {
                ReflectionDictionary reflectionDictionary = _dataStoreManager.GetDictionary(queryClassInfo);
                reflectionDictionary.QueryClassInfo(queryClassInfo.ClassType);
            }
        }

    }
}