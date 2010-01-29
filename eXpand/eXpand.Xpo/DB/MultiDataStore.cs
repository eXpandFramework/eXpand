using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;

namespace eXpand.Xpo.DB {
    public class MultiDataStore {
        readonly DataStoreManager _dataStoreManager;

        public MultiDataStore(string connectionString, XPDictionary xpDictionary) {
            _dataStoreManager = new DataStoreManager(connectionString);
            FillDictionaries(xpDictionary);
        }

        void FillDictionaries(XPDictionary xpDictionary){
            foreach (XPClassInfo queryClassInfo in xpDictionary.Classes.OfType<XPClassInfo>().Where(info => !(info is IntermediateClassInfo))){
                ReflectionDictionary reflectionDictionary = _dataStoreManager.GetDictionary(queryClassInfo);
                reflectionDictionary.QueryClassInfo(queryClassInfo.ClassType);
            }
        }

        public void UpdateSchema(DataStoreUpdateSchemaEventArgs args) {
            foreach (KeyValuePair<IDataStore, List<DBTable>> dataStore in _dataStoreManager.GetDataStores(args.Tables)) {
                IDataStore store = dataStore.Key;
                List<DBTable> dbTables = dataStore.Value;
                
                store.UpdateSchema(false, dbTables.ToArray());
            }
            args.Updated = true;
        }

        public void SelectData(DataStoreSelectDataEventArgs args) {
            var resultSet = new List<SelectStatementResult>();
            foreach (SelectedData selectedData in args.SelectStatements.Select(
                stm => _dataStoreManager.SimpleDataLayers[_dataStoreManager.GetKey(stm.TableName)].SelectData(stm)).Where(
                selectedData => selectedData != null)) {
                resultSet.AddRange(selectedData.ResultSet);
            }
            args.SelectData = new SelectedData(resultSet.ToArray());
        }

        public void ModifyData(DataStoreModifyDataEventArgs args) {
            var modificationResultIdentities = new List<ParameterValue>();
            foreach (ModificationStatement stm in args.ModificationStatements) {
                if (stm.TableName == "XPObjectType") {
                    foreach (var dataLayer in _dataStoreManager.SimpleDataLayers.Select(pair => pair.Value)){
                        modificationResultIdentities.AddRange(dataLayer.ModifyData(stm).Identities);    
                    }
                }
                else {
                    string key = _dataStoreManager.GetKey(stm.TableName);
                    ModificationResult modificationResult = _dataStoreManager.SimpleDataLayers[key].ModifyData(stm);
                    if (modificationResult != null){
                        modificationResultIdentities.AddRange(modificationResult.Identities);
                    }   
                }
            }
            args.ModificationResult = new ModificationResult(modificationResultIdentities);
        }
    }
}