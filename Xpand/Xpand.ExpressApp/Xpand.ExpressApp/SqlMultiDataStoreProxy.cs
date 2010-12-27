using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.Xpo;
using Xpand.Xpo.DB;


namespace Xpand.ExpressApp {
    public class SqlMultiDataStoreProxy : SqlDataStoreProxy {
        readonly XpoObjectHacker _xpoObjectHacker=new XpoObjectHacker();
        readonly DataStoreManager _dataStoreManager;

        protected SqlMultiDataStoreProxy() {
        }

        public SqlMultiDataStoreProxy(string connectionString)
            : this(connectionString, XafTypesInfo.XpoTypeInfoSource.XPDictionary) {

        }

        public SqlMultiDataStoreProxy(string connectionString, XPDictionary xpDictionary)
            : base(connectionString) {
            _dataStoreManager = new DataStoreManager(connectionString);
            FillDictionaries(xpDictionary);
        }


        public DataStoreManager DataStoreManager {
            get { return _dataStoreManager; }
        }

        void FillDictionaries(XPDictionary xpDictionary) {
            foreach (XPClassInfo queryClassInfo in xpDictionary.Classes.OfType<XPClassInfo>().Where(info => !(info is IntermediateClassInfo))) {
                ReflectionDictionary reflectionDictionary = _dataStoreManager.GetDictionary(queryClassInfo);
                reflectionDictionary.QueryClassInfo(queryClassInfo.ClassType);
            }
        }

        public override ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
            var modificationResultIdentities = new List<ParameterValue>();
            var dataStoreModifyDataEventArgs = new DataStoreModifyDataEventArgs(dmlStatements);
            OnDataStoreModifyData(dataStoreModifyDataEventArgs);
            foreach (ModificationStatement stm in dataStoreModifyDataEventArgs.ModificationStatements) {
                if (stm.TableName == typeof(XPObjectType).Name) {
                    ModifyXPObjectTypeData(stm, modificationResultIdentities);
                } else {
                    ModifyData(stm, modificationResultIdentities);
                }
            }
            return new ModificationResult(modificationResultIdentities);
        }

        void ModifyData(ModificationStatement stm, List<ParameterValue> modificationResultIdentities) {
            string key = _dataStoreManager.GetKey(stm.TableName);
            ModificationResult modificationResult = _dataStoreManager.SimpleDataLayers[key].ModifyData(stm);
            if (modificationResult != null) {
                modificationResultIdentities.AddRange(modificationResult.Identities);
            }
        }

        void ModifyXPObjectTypeData(ModificationStatement modificationStatement, List<ParameterValue> modificationResultIdentities) {
            var insertStatement = (InsertStatement)modificationStatement;
            foreach (var parameterValues in _dataStoreManager.SimpleDataLayers.Select(pair
                => pair.Value).Select(dataLayer => ModifyXPObjectTypeDataCore(dataLayer, insertStatement)).Where(values => values != null)) {
                modificationResultIdentities.AddRange(parameterValues);
            }
        }

        ParameterValue[] ModifyXPObjectTypeDataCore(SimpleDataLayer dataLayer, InsertStatement insertStatement) {
            if (TypeExists(dataLayer, insertStatement)) return null;
            if (!IsMainLayer(dataLayer.Connection)) {
                _xpoObjectHacker.CreateObjectTypeIndetifier(insertStatement,_dataStoreManager.SimpleDataLayers[DataStoreManager.STR_Default]);
            }
            return dataLayer.ModifyData(insertStatement).Identities;
        }



        bool TypeExists(SimpleDataLayer dataLayer, InsertStatement stm1) {
            if (IsMainLayer(dataLayer.Connection))
                return false;
            var session = new Session(dataLayer) { IdentityMapBehavior = IdentityMapBehavior.Strong };
            var value = stm1.Parameters.ToList()[0].Value as string;
            var xpObjectType = session.FindObject<XPObjectType>(type => type.TypeName == value);
            return xpObjectType != null;
        }

        bool IsMainLayer(IDbConnection connection) {
            return connection.ConnectionString == Connection.ConnectionString;
        }

        public override SelectedData SelectData(params SelectStatement[] selects) {
            var resultSet = new List<SelectStatementResult>();
            List<SelectedData> selectedDatas = selects.Select(stm => {
                OnDataStoreSelectData(new DataStoreSelectDataEventArgs(new[] { stm }));
                return _dataStoreManager.SimpleDataLayers[_dataStoreManager.GetKey(stm.TableName)].SelectData(stm);
            }).ToList();
            foreach (SelectedData selectedData in selectedDatas.Where(
                selectedData => selectedData != null)) {
                resultSet.AddRange(selectedData.ResultSet);
            }
            return new SelectedData(resultSet.ToArray());
        }

        public override UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
            foreach (KeyValuePair<IDataStore, List<DBTable>> dataStore in _dataStoreManager.GetDataStores(tables)) {
                var store = (ConnectionProviderSql)dataStore.Key;
                List<DBTable> dbTables = dataStore.Value;
                if (!IsMainLayer(store.Connection))
                    _xpoObjectHacker.EnsureIsNotIdentity(dbTables);
                store.UpdateSchema(false, dbTables.ToArray());
                RunExtraUpdaters(tables, store, dontCreateIfFirstTableNotExist);
            }
            return UpdateSchemaResult.SchemaExists;
        }

        void RunExtraUpdaters(DBTable[] tables, ConnectionProviderSql store, bool dontCreateIfFirstTableNotExist) {
            foreach (var schemaUpdater in schemaUpdaters) {
                schemaUpdater.Update(store, new DataStoreUpdateSchemaEventArgs(dontCreateIfFirstTableNotExist, tables));
            }
        }
    }
}