using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;

namespace Xpand.Xpo.DB {
    [Obsolete]
    public class MultiDataStore {
        readonly DataStoreManager _dataStoreManager;

        public MultiDataStore(string connectionString, XPDictionary xpDictionary) {
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

        public void UpdateSchema(DataStoreUpdateSchemaEventArgs args) {
            foreach (KeyValuePair<IDataStore, DataStoreInfo> dataStore in _dataStoreManager.GetDataStores(args.Tables)) {
                IDataStore store = dataStore.Key;
                List<DBTable> dbTables = dataStore.Value.DbTables;

                store.UpdateSchema(false, dbTables.ToArray());
            }
            args.Updated = true;
        }

        public void SelectData(DataStoreSelectDataEventArgs args) {
            var resultSet = new List<SelectStatementResult>();
            List<SelectedData> selectedDatas = args.SelectStatements.Select(stm =>
                    _dataStoreManager.SimpleDataLayers[_dataStoreManager.GetKeyInfo(stm.TableName)].SelectData(stm)).ToList();
            foreach (SelectedData selectedData in selectedDatas.Where(
                selectedData => selectedData != null)) {
                resultSet.AddRange(selectedData.ResultSet);
            }
            args.SelectedData = new SelectedData(resultSet.ToArray());
        }

        public void ModifyData(DataStoreModifyDataEventArgs args) {
            var modificationResultIdentities = new List<ParameterValue>();
            foreach (ModificationStatement stm in args.ModificationStatements) {
                if (stm.TableName == "XPObjectType") {
                    foreach (var dataLayer in _dataStoreManager.SimpleDataLayers.Select(pair => pair.Value)) {
                        modificationResultIdentities.AddRange(dataLayer.ModifyData(stm).Identities);
                    }
                } else {
                    string key = _dataStoreManager.GetKeyInfo(stm.TableName);
                    ModificationResult modificationResult = _dataStoreManager.SimpleDataLayers[key].ModifyData(stm);
                    if (modificationResult != null) {
                        modificationResultIdentities.AddRange(modificationResult.Identities);
                    }
                }
            }
            args.ModificationResult = new ModificationResult(modificationResultIdentities);
        }

        public SimpleDataLayer GetDataLayer(XPDictionary xpDictionary, MultiDataStore multiDataStore, Type type) {
            string connectionString = multiDataStore.DataStoreManager.GetConnectionString(type);
            var xpoDataStoreProxy = new DataStoreProxy(connectionString);
            xpoDataStoreProxy.DataStoreModifyData += (o, eventArgs) => multiDataStore.ModifyData(eventArgs);
            xpoDataStoreProxy.DataStoreSelectData += (sender1, dataEventArgs) => {
                if (multiDataStore.DataStoreManager.SimpleDataLayers.Count > 1 && IsQueryingXPObjectType(dataEventArgs)) {
                    createExcludeXPObjectTypeArgs(dataEventArgs.SelectStatements, xpDictionary);
                }
                multiDataStore.SelectData(dataEventArgs);
            };
            xpoDataStoreProxy.DataStoreUpdateSchema += (o1, schemaEventArgs) => multiDataStore.UpdateSchema(schemaEventArgs);
            return new SimpleDataLayer(xpDictionary, xpoDataStoreProxy);

        }
        void createExcludeXPObjectTypeArgs(IEnumerable<SelectStatement> selectStatements, XPDictionary xpDictionary) {
            var typeNames = xpDictionary.Classes.OfType<XPClassInfo>().Where(classInfo => classInfo.ClassType != null).Select(info => info.ClassType.FullName);
            foreach (var selectStatement in selectStatements.Where(statement => statement.TableName == "XPObjectType")) {
                List<string> values = typeNames.ToList();
                var criteriaOperator = new GroupOperator(GroupOperatorType.Or);
                foreach (var value in values) {
                    criteriaOperator.Operands.Add(new QueryOperand("TypeName", selectStatement.Alias) == value);
                }
                selectStatement.Condition = criteriaOperator;
            }
        }

        bool IsQueryingXPObjectType(DataStoreSelectDataEventArgs dataEventArgs) {
            return dataEventArgs.SelectStatements.Select(statement => statement.TableName).Where(s => s == "XPObjectType").FirstOrDefault() != null;
        }
    }
}