using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.Xpo;
using Xpand.Xpo.DB;


namespace Xpand.ExpressApp {
    public class MultiDataStoreProxy : DataStoreProxy {
        readonly XpoObjectHacker _xpoObjectHacker = new XpoObjectHacker();
        readonly DataStoreManager _dataStoreManager;

        protected MultiDataStoreProxy() {
        }

        public MultiDataStoreProxy(string connectionString)
            : this(connectionString, XpandModuleBase.Dictiorary) {

        }

        public MultiDataStoreProxy(string connectionString, XPDictionary xpDictionary)
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
            var dataStoreModifyDataEventArgs = new DataStoreModifyDataEventArgs(dmlStatements);
            OnDataStoreModifyData(dataStoreModifyDataEventArgs);
            var name = typeof(XPObjectType).Name;
            var insertStatement = dataStoreModifyDataEventArgs.ModificationStatements.OfType<InsertStatement>().FirstOrDefault(statement => statement.TableName == name);
            var modificationResult = new ModificationResult();
            if (insertStatement != null) {
                modificationResult = ModifyXPObjectTable(dmlStatements, insertStatement, modificationResult);
            } else {
                var key = _dataStoreManager.GetKeyInfo(dmlStatements[0].TableName);
                modificationResult = _dataStoreManager.SimpleDataLayers[key].ModifyData(dmlStatements);
            }
            if (modificationResult != null) return modificationResult;
            throw new NotImplementedException();
        }

        ModificationResult ModifyXPObjectTable(ModificationStatement[] dmlStatements, InsertStatement insertStatement, ModificationResult modificationResult) {
            foreach (var simpleDataLayer in _dataStoreManager.SimpleDataLayers) {
                if (!simpleDataLayer.Value.IsLegacy) {
                    var dataLayer = simpleDataLayer.Value;
                    if (!TypeExists(dataLayer, insertStatement)) {
                        if (!dataLayer.IsMainLayer) {
                            _xpoObjectHacker.CreateObjectTypeIndetifier(insertStatement, _dataStoreManager.SimpleDataLayers[DataStoreManager.StrDefault]);
                        }
                        var modifyData = dataLayer.ModifyData(dmlStatements);
                        if (modifyData.Identities.Any())
                            modificationResult = modifyData;
                    }
                }
            }
            return modificationResult;
        }


        bool TypeExists(DataStoreManagerSimpleDataLayer dataLayer, InsertStatement stm1) {
            if (dataLayer.IsMainLayer)
                return false;
            var session = new Session(dataLayer) { IdentityMapBehavior = IdentityMapBehavior.Strong };
            var value = stm1.Parameters.ToList()[0].Value as string;
            var xpObjectType = session.FindObject<XPObjectType>(type => type.TypeName == value);
            return xpObjectType != null;
        }

        bool IsMainLayer(IDbConnection connection) {
            return (connection == null || Connection == null) || connection.ConnectionString == Connection.ConnectionString;
        }

        public override SelectedData SelectData(params SelectStatement[] selects) {
            var resultSet = new List<SelectStatementResult>();
            List<SelectedData> selectedDatas = selects.Select(stm => {
                OnDataStoreSelectData(new DataStoreSelectDataEventArgs(new[] { stm }));
                var simpleDataLayer = _dataStoreManager.SimpleDataLayers[_dataStoreManager.GetKeyInfo(stm.TableName)];
                return simpleDataLayer.SelectData(stm);
            }).ToList();
            foreach (SelectedData selectedData in selectedDatas.Where(
                selectedData => selectedData != null)) {
                resultSet.AddRange(selectedData.ResultSet);
            }
            return new SelectedData(resultSet.ToArray());
        }

        public override UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
            foreach (KeyValuePair<IDataStore, DataStoreInfo> keyValuePair in _dataStoreManager.GetDataStores(tables)) {
                var store = keyValuePair.Key as ConnectionProviderSql;
                if (store != null) {
                    var dataStoreInfo = keyValuePair.Value;
                    var storeInfo = dataStoreInfo;
                    List<DBTable> dbTables = storeInfo.DbTables;
                    if (Connection == null)
                        throw new NullReferenceException();
                    if (!storeInfo.IsLegacy && !IsMainLayer(store.Connection))
                        _xpoObjectHacker.EnsureIsNotIdentity(dbTables);
                    if (!(storeInfo.IsLegacy && dbTables.Count == 1 && dbTables[0].Name == typeof(XPObjectType).Name))
                        store.UpdateSchema(false, dbTables.ToArray());
                    RunExtraUpdaters(tables, store, dontCreateIfFirstTableNotExist);
                }
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