using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using eXpand.Xpo.DB;

namespace eXpand.ExpressApp {
    public class XpoMultiDataStoreProxy:XpoDataStoreProxy {
        readonly DataStoreManager _dataStoreManager;

        protected XpoMultiDataStoreProxy() {
        }

        public XpoMultiDataStoreProxy(string connectionString) : base(connectionString) {
            _dataStoreManager = new DataStoreManager(connectionString);
            FillDictionaries(XafTypesInfo.XpoTypeInfoSource.XPDictionary);
        }

        public DataStoreManager DataStoreManager
        {
            get { return _dataStoreManager; }
        }

        void FillDictionaries(XPDictionary xpDictionary){
            foreach (XPClassInfo queryClassInfo in xpDictionary.Classes.OfType<XPClassInfo>().Where(info => !(info is IntermediateClassInfo))){
                ReflectionDictionary reflectionDictionary = _dataStoreManager.GetDictionary(queryClassInfo);
                reflectionDictionary.QueryClassInfo(queryClassInfo.ClassType);
            }
        }

        public override ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
            var modificationResultIdentities = new List<ParameterValue>();
            var dataStoreModifyDataEventArgs = new DataStoreModifyDataEventArgs(dmlStatements);
            OnDataStoreModifyData(dataStoreModifyDataEventArgs);
            foreach (ModificationStatement stm in dataStoreModifyDataEventArgs.ModificationStatements){
                if (stm.TableName == typeof(XPObjectType).Name) {
                    var stm1 = stm;
                    foreach (var parameterValues in _dataStoreManager.SimpleDataLayers.Select(pair 
                        => pair.Value).Select(dataLayer => dataLayer.ModifyData(stm1).Identities)) {
                        modificationResultIdentities.AddRange(parameterValues);
                    }
                }
                else {
                    string key = _dataStoreManager.GetKey(stm.TableName);
                    ModificationResult modificationResult = _dataStoreManager.SimpleDataLayers[key].ModifyData(stm);
                    if (modificationResult != null) {
                        modificationResultIdentities.AddRange(modificationResult.Identities);
                    }
                }
            }
            return new ModificationResult(modificationResultIdentities);
        }

        public override SelectedData SelectData(params SelectStatement[] selects) {
            var resultSet = new List<SelectStatementResult>();
            List<SelectedData> selectedDatas = selects.Select(stm => {
                OnDataStoreSelectData(new DataStoreSelectDataEventArgs(new[]{stm}));
                return _dataStoreManager.SimpleDataLayers[_dataStoreManager.GetKey(stm.TableName)].SelectData(stm);
            }).ToList();
            foreach (SelectedData selectedData in selectedDatas.Where(
                selectedData => selectedData != null)) {
                resultSet.AddRange(selectedData.ResultSet);
            }
            return new SelectedData(resultSet.ToArray());
        }
        public override UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables){
            foreach (KeyValuePair<IDataStore, List<DBTable>> dataStore in _dataStoreManager.GetDataStores(tables)){
                IDataStore store = dataStore.Key;
                List<DBTable> dbTables = dataStore.Value;
                store.UpdateSchema(false, dbTables.ToArray());
            }
            return UpdateSchemaResult.SchemaExists;
        }

        public SimpleDataLayer GetDataLayer(XPDictionary xpDictionary, MultiDataStore multiDataStore, Type type) {
            string connectionString = multiDataStore.DataStoreManager.GetConnectionString(type);
            var xpoDataStoreProxy = new XpoDataStoreProxy(connectionString);
            xpoDataStoreProxy.DataStoreModifyData += (o, eventArgs) => multiDataStore.ModifyData(eventArgs);
            xpoDataStoreProxy.DataStoreSelectData += (sender1, dataEventArgs) => {
                if (multiDataStore.DataStoreManager.SimpleDataLayers.Count > 1 && IsQueryingXPObjectType(dataEventArgs)) {
                    createExcludeXPObjectTypeArgs(dataEventArgs.SelectStatements, xpDictionary);
                }
                multiDataStore.SelectData(dataEventArgs);
            };
            xpoDataStoreProxy.DataStoreUpdateSchema +=
                (o1, schemaEventArgs) => multiDataStore.UpdateSchema(schemaEventArgs);
            return new SimpleDataLayer(xpDictionary, xpoDataStoreProxy);
        }

        void createExcludeXPObjectTypeArgs(IEnumerable<SelectStatement> selectStatements, XPDictionary xpDictionary) {
            IEnumerable<string> typeNames =
                xpDictionary.Classes.OfType<XPClassInfo>().Where(classInfo => classInfo.ClassType != null).Select(
                    info => info.ClassType.FullName);
            foreach (
                SelectStatement selectStatement in
                    selectStatements.Where(statement => statement.TableName == "XPObjectType")) {
                List<string> values = typeNames.ToList();
                var criteriaOperator = new GroupOperator(GroupOperatorType.Or);
                foreach (string value in values) {
                    criteriaOperator.Operands.Add(new QueryOperand("TypeName", selectStatement.Alias) == value);
                }
                selectStatement.Condition = criteriaOperator;
            }
        }

        bool IsQueryingXPObjectType(DataStoreSelectDataEventArgs dataEventArgs) {
            return
                dataEventArgs.SelectStatements.Select(statement => statement.TableName).Where(s => s == "XPObjectType").
                    FirstOrDefault() != null;
        }
    }
}